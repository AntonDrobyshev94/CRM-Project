cd /d "D:\kafka_2.12-3.8.0\bin\windows"
timeout 1
kafka-topics.bat --create --topic auth-top --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1