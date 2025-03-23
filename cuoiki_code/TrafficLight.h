#ifndef TrafficLight_h
#define TrafficLight_h

#include <Arduino.h>

class TrafficLight
{
public:
    TrafficLight(int greenPin, int yellowPin, int redPin, int greenPinInterval, int yellowPinInterval, int redPinInterval);
    void begin();
    void greenPinActivate();
    void yellowPinActivate();
    void redPinActivate();
    void pinDeactivate();
    int getGreenPinInterval();
    int getYellowPinInterval();
    int getRedPinInterval();
    void peakHourMode();
    void normalInterval();
    void updateIntervals(int greenInterval, int yellowInterval, int redInterval);

private:
    int _greenPin, _yellowPin, _redPin;
    int _greenPinInterval, _yellowPinInterval, _redPinInterval;
    int _greenTmpInterval, _yellowTmpInterval, _redTmpInterval;
};

#endif
