using Twilio.TwiML;
using Twilio.AspNet.Core;
using Twilio.AspNet.Common;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();


string toNumber = config["AppSettings:PhoneNumber"];

//  default time values set here. Can be changed in appsettings.json
string startDay= config["AppSettings:DayStart"];
string endDay = config["AppSettings:DayEnd"];
int startTime = 8;
int endTime =18;
int.TryParse(startDay, out startTime);
int.TryParse(endDay, out endTime);

app.MapPost("/voice", () => 
{

	var now = DateTime.Now.TimeOfDay;
	var start = new TimeSpan(startTime,0,0); 
	var end = new TimeSpan(endTime,0, 0); 
   	var response = new VoiceResponse();


		if ((now > start) && (now < end))
		{
		    // Called within the right time, so forward this call
            // Phone number can be set in appsettings
			response.Say("Please wait while we forward your call.");
		    response.Dial(toNumber);
		}

		else
		{
			response.Redirect (url: new Uri("/voice/mail", UriKind.Relative));
		}

   return Results.Extensions.TwiML(response);
});

app.MapPost("/voice/mail", ([FromForm] VoiceRequest request)

 => 
{
    var response = new VoiceResponse();
    response.Say("Hello. You've called after our working hours. Please leave a message after the beep.");
    response.Record(
        timeout: 10
    );

return Results.Extensions.TwiML(response);
})
.DisableAntiforgery();


app.Run();
