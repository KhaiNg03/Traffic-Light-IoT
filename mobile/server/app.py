import serial
from flask import Flask, request, jsonify
from flask_cors import CORS, cross_origin
import socket
import json
import time
import threading
import queue
import sqlite3

app = Flask(__name__)
CORS(app)

ser = None
command_queue = queue.Queue()
data_queue = queue.Queue()
db_queue = queue.Queue()
buffer = ""  # Bộ đệm lưu trữ dữ liệu tạm thời
receiving_data = False # Biến cờ

def read_serial_data():
    global ser, data_queue, buffer, receiving_data
    while True:
        if ser is not None and ser.is_open:
            try:
                line = ser.readline().decode('utf-8', errors='ignore').strip()
                if line:
                    if line == "END":
                        receiving_data = False
                        try:
                            data = json.loads(buffer)
                            if data_queue.qsize() > 1:
                                data_queue.get()
                            data_queue.put(data)
                            buffer = "" # Xóa bộ đệm sau khi xử lý
                        except json.JSONDecodeError as e:
                            app.logger.error(f"Invalid JSON: {buffer}, Error: {e}")
                            buffer = "" # Xóa bộ đệm nếu không hợp lệ
                    elif receiving_data:
                        buffer += line
                    else:
                        receiving_data = True
                        buffer = line
            except serial.SerialException as e:
                app.logger.error(f"Serial communication error: {e}")
                if ser.is_open:
                    ser.close()
                    ser = None
                break
            except Exception as e:
                app.logger.error(f"Error reading serial data: {e}")
                break
            time.sleep(0.1)
        else:
            time.sleep(1)
            
def send_commands():
    global ser, command_queue
    while True:
        if ser is not None and ser.is_open:
            try:
                command = command_queue.get()
                ser.write((command + '\n').encode())
                command_queue.task_done()
            except serial.SerialException as e:
                app.logger.error(f"Serial communication error while sending command: {e}")
                if ser.is_open:
                    ser.close()
                    ser = None
                break
            except Exception as e:
                app.logger.error(f"Error sending command: {e}")
                break
        time.sleep(0.1)

def db_worker():
    conn_obj = None
    try:
        conn_obj = sqlite3.connect('traffic_light.db')
        cursor_obj = conn_obj.cursor()
        cursor_obj.execute("DROP TABLE IF EXISTS traffic_commands")
        cursor_obj.execute("""
            CREATE TABLE IF NOT EXISTS traffic_commands (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                command TEXT,
                timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
            )
        """)
        conn_obj.commit()
        while True:
            try:
                opt, args = db_queue.get()
                if opt == "insert":
                    cursor_obj.execute("INSERT INTO traffic_commands (command) VALUES (?)", args)
                    conn_obj.commit()
                # elif opt == "select":
                #     cursor_obj.execute("SELECT command FROM traffic_commands ORDER BY id DESC LIMIT 1")
                #     result = cursor_obj.fetchone()
                #     db_queue.task_done()
                #     if result:
                #         yield result[0]
                #     else:
                #         yield None
                else:
                    raise ValueError(f"Unknown database operation: {opt}")
                db_queue.task_done()
                
                cursor_obj.execute("SELECT command FROM traffic_commands ORDER BY id DESC LIMIT 1")
                result = cursor_obj.fetchone()
                print(result[0])
            except sqlite3.Error as e:
                app.logger.error(f"Database error: {e}")
            except Exception as e:
                app.logger.exception(f"Error in database worker: {e}")
    except Exception as e:
        app.logger.exception(f"Error connecting to the database: {e}")
    finally:
        if conn_obj:
            conn_obj.close()

@app.route('/control', methods=['POST'])
def control_arduino():
    global command_queue
    try:
        data = request.get_json()
        command = data.get('command')
        if command:
            command_queue.put(command)
            db_queue.put(("insert", (command,)))
            return jsonify({'message': f'Command {command} sent successfully'}), 200
        else:
            return jsonify({'error': 'No command provided'}), 400

    except Exception as e:
        app.logger.exception(f"Error processing control command: {e}")
        return jsonify({'error': 'Failed to send command'}), 500

@app.route('/data', methods=['GET'])
def get_data():
    global ser, data_queue
    if ser is not None and ser.is_open:
        try:
            data = data_queue.get(timeout=1)
            return jsonify({'data': data}), 200
        except queue.Empty as e:
            return jsonify({'error': 'No data available'}), 204 # 204 No Content
        except Exception as e:
            app.logger.error(f"Unexpected error: {e}")
            return jsonify({'error': 'Unexpected error occurred'}), 500

def get_ipv4():
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        s.connect(("8.8.8.8", 80))  # Kết nối đến một server DNS công cộng
        ip = s.getsockname()[0]
        s.close()
        return ip
    except Exception as e:
        app.logger.error(f"Error getting IPv4 address: {e}")
        return "127.0.0.1"  # Default localhost

if __name__ == '__main__':
    try:
        ser = serial.Serial(port='COM2', baudrate=9600, timeout=1)
        
        read_data_thread = threading.Thread(target=read_serial_data, daemon=True)
        read_data_thread.start()
        
        command_thread = threading.Thread(target=send_commands, daemon=True)
        command_thread.start()
        
        db_thread = threading.Thread(target=db_worker, daemon=True)
        db_thread.start()
        
        ip_address = get_ipv4()
        app.run(debug=True, host=ip_address, port=5000, use_reloader=False, threaded=True)
    except serial.SerialException as e:
        app.logger.error(f'Serial communication error: {e}')
    except Exception as e:
        app.logger.error(f"An unexpected error occurred: {e}")
    finally:
        if ser is not None and ser.is_open:
            ser.close()
            app.logger.info("Serial port closed.")
