using Navigation_Service;
using Serilog;

ILogger logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();


var gpsSource = new UdpNmeaSource(Constants.GNSS_PORT, logger);
var gpsDevice = new GNSSDevice(logger);
gpsDevice.ConnectSource(gpsSource);
var devices = new List<INavigationDevice> { gpsDevice };

NavigationManager navigationManager = new NavigationManager(logger, devices);
navigationManager.run();

