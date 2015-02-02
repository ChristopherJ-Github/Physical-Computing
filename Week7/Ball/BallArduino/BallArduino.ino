#include <Servo.h> 

int incomingByte;   
Servo myservo;
int pos = 0;

void setup() {

  Serial.begin(9600);
  myservo.attach(9);
  myservo.write(0);
}

void loop() {
 
  if (Serial.available() > 0) {

    incomingByte = Serial.read();

    if (incomingByte == 'H') {

      myservo.write(180); 
      delay (15);  
    }
    
    if (incomingByte == 'L') {

      myservo.write(0); 
      delay (15);  
    }
  }
}
