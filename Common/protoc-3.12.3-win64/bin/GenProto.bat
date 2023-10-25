protoc.exe -I=./ --csharp_out=./ ./Protocol.proto 
IF ERRORLEVEL 1 PAUSE

START ../../../Server/PacketGenerator/bin/PacketGenerator.exe ./Protocol.proto
XCOPY /Y Protocol.cs "../../../Client/Assets/Scripts/Packet"
XCOPY /Y Protocol.cs "../../../Server/Server/Packet"
XCOPY /Y Protocol.cs "../../../Server/DungeonServer/Packet"
XCOPY /Y Protocol.cs "../../../Server/MatchingServer/Packet"
XCOPY /Y Protocol.cs "../../../Server/DummyClient/Packet"
XCOPY /Y ClientPacketManager.cs "../../../Client/Assets/Scripts/Packet"
XCOPY /Y ServerPacketManager.cs "../../../Server/Server/Packet"
XCOPY /Y ServerPacketManager.cs "../../../Server/DungeonServer/Packet"
XCOPY /Y ServerPacketManager.cs "../../../Server/MatchingServer/Packet"
XCOPY /Y ClientPacketManager.cs "../../../Server/DummyClient/Packet"