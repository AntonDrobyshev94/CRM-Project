cd /d "D:\kafka_2.12-3.8.0\bin\windows"
timeout 1
call zookeeper-server-stop.bat
timeout 3
call kafka-server-stop.bat


