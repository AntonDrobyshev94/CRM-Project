cd /d "D:\kafka_2.12-3.8.0\bin\windows"
kafka-topics.bat --create --topic response-topic --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1