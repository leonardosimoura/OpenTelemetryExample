# Running

docker run -d -p 9411:9411 --name zipkin --restart always openzipkin/zipkin

docker run -d --name jaeger -e COLLECTOR_ZIPKIN_HOST_PORT=:9412 -p 5775:5775/udp -p 6831:6831/udp   -p 6832:6832/udp   -p 5778:5778   -p 16686:16686   -p 14268:14268   -p 14250:14250  -p 9412:9412 --restart always  jaegertracing/all-in-one:1.27

docker run --name postgres --restart=always -e POSTGRES_DB="myuser" -e POSTGRES_USER="myuser" -e POSTGRES_PASSWORD="123456789" -p 5432:5432 -d postgres

docker run -d --name mongodb  --restart=always -e MONGO_INITDB_ROOT_USERNAME=myuser -e MONGO_INITDB_ROOT_PASSWORD=123456789 -p 27017:27017 mongo

# Endpoints

### Zipkin

http://localhost:9411/zipkin/

### Jaeger

http://localhost:16686/

# Clean Up

docker rm zipkin --force

docker rm jaeger --force

docker rm postgres --force

docker rm mongodb --force