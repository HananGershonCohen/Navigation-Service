
using Navigation_Service;

//NavigationManager navigationManager = new NavigationManager();
//navigationManager.run();

UdpReceiver udpReceiver = new UdpReceiver(11000);
udpReceiver.StartListening();
Console.ReadLine();