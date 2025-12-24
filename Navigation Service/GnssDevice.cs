using System;
using System.Collections.Generic;
using NmeaParser.Messages;

namespace Navigation_Service
{
    internal class GnssDevice : NavigationDevicesBase
    {
        private GPSPosition _currentPosition = new GPSPosition();

    
        private readonly Dictionary<Type, INmeaMapper> _mappers;

        public GnssDevice()
        {
            _mappers = new Dictionary<Type, INmeaMapper>
            {
                { typeof(Gga), new GgaMapper() },
                // אפשר להוסיף כאן בקלות:
                // { typeof(Rmc), new RmcMapper() },
                // { typeof(Vtg), new VtgMapper() },
            };
        }

        // function to disconnect from source
        public void ConnectSource(INmeaSource source)
        {
            source.MessageReceived += OnNmeaMessageReceived;
            source.Start();
        }

        // הפונקציה המרכזית שמקבלת החלטות
        private void OnNmeaMessageReceived(object sender, NmeaMessage message)
        {
            Type msgType = message.GetType();

            // בדיקה האם יש לנו Mapper רשום עבור סוג ההודעה הזה
            if (_mappers.TryGetValue(msgType, out var mapper))
            {
                // הפעלת הלוגיקה הספציפית
                mapper.Map(message, _currentPosition);

                // לוג או עדכון מערכת
                Console.WriteLine($"[GNSS] Pos Updated: Lat={_currentPosition.Latitude:F6}, Lon={_currentPosition.Longitude:F6} (Src: {msgType.Name})");
            }
            // else: הודעה שאין לנו עניין בה (כמו GSV), מתעלמים בשקט
        }
    }
}