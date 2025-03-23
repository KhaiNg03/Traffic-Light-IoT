import { View, Button, StyleSheet, Alert } from 'react-native';
import React, { useState, useEffect } from 'react';
import TrafficLight from './TrafficLight';

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'space-between',
    alignItems: 'center',
    flexDirection: 'column',
    marginVertical: 100,
  },
  trafficLightSection: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    width: '100%',
  },
  buttonSection: {
  },
  buttonContainer: {
    marginBottom: 20,
    width: '70%',
  },
});

const App = () => {
  const [data, setData] = useState({
    light1: '',
    light2: '',
    counter1: 0,
    counter2: 0,
  });
  const featureList = [
    { command: 2, name: 'Night' },
    { command: 3, name: 'Normal' },
    { command: 4, name: 'Peak hour' },
    { command: 5, name: 'Manual' },
  ];
  const [blinking, setBlinking] = useState(false);
  const [yellowState, setYellowState] = useState(false);
  const [isManualFirstTime, setIsManualFirstTime] = useState(true);
  const BASE_URL = '192.168.2.89:5000'

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch(`http://${BASE_URL}/data`);

        if (response.status === 204) {
          return;
        }

        if (!response.ok) {
          const errorBody = await response.text();
          throw new Error(`HTTP error! status: ${response.status}, body: ${errorBody}`);
        }

        const json = await response.json(); console.log(json.data)
        setData(json.data);
      } catch (error) {
        console.error('Error fetching data:', error);
      }
    };
    const intervalId = setInterval(fetchData, 1000);
    return () => clearInterval(intervalId);
  }, [])

  useEffect(() => {
    let intervalId;
    if (blinking) {
      intervalId = setInterval(() => {
        setYellowState(prev => !prev);
      }, 500);
    } else {
      setYellowState(false);
    }

    return () => clearInterval(intervalId);
  }, [blinking]);

  const handleManualMode = () => {
    if (isManualFirstTime) {
      setData({ light1: 'red', light2: 'green' });
      setIsManualFirstTime(false);
    } else {
      setData(prev => ({ light1: prev.light2, light2: prev.light1, }));
    }
  };

  const sendCommand = async (command) => {
    try {
      const response = await fetch(`http://${BASE_URL}/control`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ command }),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const commandResponse = await response.json();
      Alert.alert('Success', commandResponse.message);

      switch (command) {
        case '2': // Night
          setData({ light1: '', light2: '' });
          setBlinking(true);
          break;
        case '3': // Normal
          setBlinking(false);
          setIsManualFirstTime(true);
          break;
        case '5': // Manual
          handleManualMode();
          break;
        default:
          break;
      }
    } catch (error) {
      Alert.alert('Error', error.message);
      console.error('Error:', error);
    }
  };

  return (
    <View style={styles.container}>
      <View style={styles.trafficLightSection}>
        <TrafficLight light={blinking && yellowState ? 'yellow' : data.light1} counter={data.counter1} />
        <TrafficLight light={blinking && !yellowState ? 'yellow' : data.light2} counter={data.counter2} />
      </View>
      <View style={styles.buttonSection}>
        {featureList.map((feature, index) => (
          <View style={styles.buttonContainer} key={index}>
            <Button
              onPress={() => sendCommand(feature.command.toString())}
              title={feature.name}
            />
          </View>
        ))}
      </View>
    </View>
  );
};

export default App;
