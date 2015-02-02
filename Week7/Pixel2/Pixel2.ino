int incomingByte;   

void setup() {

  Serial.begin(9600);
  myservo.attach(9);
}

void loop() {
 
  if (Serial.available() > 0) {

    incomingByte = Serial.read();

    if (incomingByte == 'H') {
      digitalWrite(ledPin, HIGH);
    }

    if (incomingByte == 'L') {
      digitalWrite(ledPin, LOW);
    }
  }
}
