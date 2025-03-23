import { View, Text, StyleSheet } from 'react-native'
import React from 'react'

const styles = StyleSheet.create({
    container: {
        flex: 1,
        flexDirection: 'column',
        justifyContent: 'space-between',
        alignItems: 'center'
    },
    trafficLight: {
        flexDirection: 'column',
        alignItems: 'center',
    },
    light: {
        width: 50,
        height: 50,
        borderRadius: 25,
        margin: 5,
        backgroundColor: 'black',
    },
    red: { backgroundColor: 'red' },
    yellow: { backgroundColor: 'yellow' },
    green: { backgroundColor: 'green' },
    counter: { fontSize: 50, marginTop: 10 },
})

const TrafficLight = ({light, counter}) => {
    return (
        <View style={styles.container}>
            <View style={styles.trafficLight}>
                <View style={[styles.light, light === 'red' ? styles.red : {}]} />
                <View style={[styles.light, light === 'yellow' ? styles.yellow : {}]} />
                <View style={[styles.light, light === 'green' ? styles.green : {}]} />
            </View>
            <Text style={styles.counter}>{counter}</Text>
        </View>
    );
}

export default TrafficLight
