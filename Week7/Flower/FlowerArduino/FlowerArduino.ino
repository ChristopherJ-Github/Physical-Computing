void setup() {
  // initialize the serial communication:
  Serial.begin(1200);
}

void loop() {

  Serial.println(analogRead(A0));
}
