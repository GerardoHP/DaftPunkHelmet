#include <SoftwareSerial.h>

SoftwareSerial BT1(4, 2); //

void setup()
{
	Serial.begin(9600);
	Serial.println("Enter AT commands:");
	BT1.begin(9600);
	pinMode(13, OUTPUT);
}

void loop()
{
	if (BT1.available())
	{
		char receivedData = BT1.read();
		Serial.write(receivedData);
		if (receivedData == '1') {
			digitalWrite(13, HIGH);
		}

		if (receivedData == '0') {
			digitalWrite(13, LOW);
		}


	}
	/*
	if (Serial.available())
	{  String S = GetLine();
	BT1.print(S);
	Serial.println("---> " + S);
	}*/
}

String GetLine()
{
	String S = "";
	if (Serial.available())
	{
		char c = Serial.read(); ;
		while (c != '\n')            //Hasta que el caracter sea intro
		{
			S = S + c;
			delay(25);
			c = Serial.read();
		}
		return (S + '\n');
	}
}