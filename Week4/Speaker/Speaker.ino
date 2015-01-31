#include "pitches.h"

int lowestValue = 1023;
int highestValue = 0;
int calibrationCounter = 300;

void setup() { 
  
  Serial.begin(9600); 
}

void loop() {
  
  int value = analogRead(0);
  calibrate(value);
  float _delay = map(value, lowestValue, highestValue, 1000/2, 1000/10);
  
  tone(8, NOTE_C4, _delay);
  delay(_delay * 1.30);
  noTone(8);
}

void calibrate (float light) {
  
  calibrationCounter --;
  if (calibrationCounter == 0)
    Serial.println("Done calibration");
  if (calibrationCounter <= 0)
     return;  
  
  if (light > highestValue) 
    highestValue = light;
  if (light < lowestValue) 
    lowestValue = light;
}
