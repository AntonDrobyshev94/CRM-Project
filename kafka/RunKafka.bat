cd /d "F:\C#\CRM\kafka_2.12-3.8.0\bin\windows"
timeout 1
start zookeeper-server-start.bat F:\C#\CRM\kafka_2.12-3.8.0\config\zookeeper.properties
timeout 2
start kafka-server-start.bat F:\C#\CRM\kafka_2.12-3.8.0\config\server.properties
timeout 3
start F:\C#\CRM\kafka_2.12-3.8.0\CreateTopic.bat
timeout 3
start F:\C#\CRM\kafka_2.12-3.8.0\CreateTopic2.bat