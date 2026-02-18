using Navigation_Service;
using Serilog;
using Serilog.Events;

ILogger logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Minute, retainedFileCountLimit: 2)
    .WriteTo.File("Logs/errors.txt", restrictedToMinimumLevel: LogEventLevel.Error, rollingInterval: RollingInterval.Minute, retainedFileCountLimit: 2)
    .CreateLogger();

// make Serilog's static API consistent with the instance
Log.Logger = logger;

try
{
    var devices = new List<INavigationDevice>();

    var gpsSource = new UdpSource(Constants.GNSS_PORT, logger);
    var nmeaParser = new Navigation_Service.NmeaParser(gpsSource, logger);
    var gpsDevice = new GNSSDevice(nmeaParser, logger);
    devices.Add(gpsDevice);

    var imuSource = new UdpSource(Constants.IMU_PORT, logger);
    var insDevice = new INSDevice(imuSource, logger);
    devices.Add(insDevice);

    NavigationManager navigationManager = new NavigationManager(logger, devices);
    navigationManager.run();
}
finally
{
    // flush and release file handles
    Log.CloseAndFlush();
}