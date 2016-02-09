#include <SoftwareSerial.h>
#include <ArduinoJson.h>
#include <Adafruit_NeoPixel.h>

#define PIN            6
#define NUMPIXELS      60
SoftwareSerial BT1(4, 2);
String colors[6];
Adafruit_NeoPixel pixels = Adafruit_NeoPixel(NUMPIXELS, PIN, NEO_GRB + NEO_KHZ800);
long color[3] = { 0,0,0 };

void setup()
{
	Serial.begin(9600);
	BT1.begin(9600);
	pinMode(13, OUTPUT);
	pixels.begin();
	Serial.println("Comenzado");
}

void loop()
{
	String receivedData = GetBTLine();
	if (receivedData != "")
	{
		Serial.println(receivedData);
		StaticJsonBuffer < 200 > jsonBuffer;
		JsonObject & root = jsonBuffer.parseObject(receivedData);

		// Test if parsing succeeds.
		if (!root.success())
		{
			return;
		}

		for (int i = 0; i < 6; i++)
		{
			String colorStr = root["colors"][i];
			Serial.println(colorStr);
			RetrieveColor(colorStr);
			for (int j = i * 10; j < (i + 1) * 10; j++)
			{
				// Serial.print(j);
				// Serial.print(" ");
				pixels.setPixelColor(j, pixels.Color(color[0], color[1], color[2])); // Moderately bright green color.
				pixels.show(); // This sends the updated pixel color to the hardware.
			}

			Serial.println("");
		}
	}
}

String GetBTLine()
{
	String str = "";
	if (BT1.available())
	{
		char c = BT1.read();
		while (c != '\n')            //Hasta que el caracter sea intro
		{
			str = str + c;
			c = BT1.read();
		}

		return (str);
	}
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

String GetCharArray(char str)
{
	String ramdomString(str);
	return ramdomString;
}

void RetrieveColor(String hexadecimal) {
	long colors[3];
	if (hexadecimal.startsWith("#")) {
		long number = (long)strtol(&hexadecimal[1], NULL, 16);
		// Split them up into r, g, b values
		color[0] = number >> 16;
		color[1] = number >> 8 & 0xFF;
		color[2] = number & 0xFF;
	}
}