# Running

Alterar as constantes de OtlpExporterEndpoint para localhost or IP Local ( elastic http://localhost:8200 / datadog http://localhost:4317)

```
docker run -d -p 9411:9411 --name zipkin --restart always openzipkin/zipkin

docker run -d --name jaeger -e COLLECTOR_ZIPKIN_HOST_PORT=:9412 -p 5775:5775/udp -p 6831:6831/udp   -p 6832:6832/udp   -p 5778:5778   -p 16686:16686   -p 14268:14268   -p 14250:14250  -p 9412:9412 --restart always  jaegertracing/all-in-one:1.27

docker run --name postgres --restart=always -e POSTGRES_DB="myuser" -e POSTGRES_USER="myuser" -e POSTGRES_PASSWORD="123456789" -p 5432:5432 -d postgres

docker run -d --name mongodb  --restart=always -e MONGO_INITDB_ROOT_USERNAME=myuser -e MONGO_INITDB_ROOT_PASSWORD=123456789 -p 27017:27017 mongo
```
## Datadog

Criar uma conta trial

Necessário criar um pipeline para o correlation de logs e traces

Necessário criar um pipeline para o Mapper da mensagem para o atributo RenderedMessage

```
docker run -d --name datadog-agent \
           -e DD_API_KEY=<API_KEY> \
           -e DD_OTLP_CONFIG_RECEIVER_PROTOCOLS_GRPC_ENDPOINT=0.0.0.0:4317 \
           -e DD_OTLP_CONFIG_RECEIVER_PROTOCOLS_HTTP_ENDPOINT=0.0.0.0:4318 \
           -e DD_LOGS_ENABLED=true \
           -e DD_LOGS_CONFIG_CONTAINER_COLLECT_ALL=true \
           -e DD_DOGSTATSD_NON_LOCAL_TRAFFIC=true \
           -p 4317:4317 -p 4318:4318 \
           -e DD_SITE="datadoghq.com" \
           -e DD_CONTAINER_EXCLUDE_LOGS="name:datadog-agent" \
           -v /var/run/docker.sock:/var/run/docker.sock:ro \
           -v /proc/:/host/proc/:ro \
           -v /opt/datadog-agent/run:/opt/datadog-agent/run:rw \
           -v /sys/fs/cgroup/:/host/sys/fs/cgroup:ro \
           datadog/agent:latest
```


## Elastic APM (K8s)

### Install ECK 

```
kubectl create -f https://download.elastic.co/downloads/eck/2.1.0/crds.yaml

kubectl apply -f https://download.elastic.co/downloads/eck/2.1.0/operator.yaml

kubectl apply -f ./eck/eck.yaml
```

### Configs

#### Kibana 

http://localhost:5601/

user elastic
senha:
    abrir o dashboad e procurar o secret "quickstart-es-elastic-user"

#### APM Integration

add apm integration

quickstart-apm-http:8200

# Endpoints

## Zipkin

http://localhost:9411/zipkin/

![Zipkin](images/zipkin_1.png)

![Zipkin](images/zipkin_2.png)

![Zipkin](images/zipkin_3.png)

## Jaeger

http://localhost:16686/

## Elastic / Kibana 

http://localhost:5601/

![Elastic](images/elastic_3.png)

![Elastic](images/elastic_5.png)

# Datadog

![Datadog](images/datadog_1.png)

![Datadog](images/datadog_2.png)


# Docker

Por hora a parte de log com o datadog está sendo feita pelo proprio agent, e o modo mais pratica para exemplificar é pelo docker

```
docker rm frontend --force

docker rm backend-pessoa --force

docker rm backend-endereco --force

docker build -t backend-endereco:latest -f BackEndEndereco\Dockerfile .

docker build -t backend-pessoa:latest -f BackEndPessoa\Dockerfile .

docker build -t frontend:latest -f FrontEnd\Dockerfile .

docker run -d --name backend-endereco -e "ConnectionStrings__BackEndEnderecoContext=mongodb://myuser:123456789@<IP>:27017/backend_endereco/?authSource=admin&readPreference=primary&ssl=false" -p 5052:80 backend-endereco:latest

docker run -d --name backend-pessoa -e "ConnectionStrings__BackEndPessoaContext=Host=<IP>;Database=backend_pessoa;Username=myuser;Password=123456789" -p 5051:80 backend-pessoa:latest

docker run -d --name frontend  -e "BackEndOptions__PessoaUrl=http://<IP>:5051"  -e "BackEndOptions__EnderecoUrl=http://<IP>:5052" -p 5053:80 frontend:latest
```


# Clean Up

```
docker rm zipkin --force

docker rm jaeger --force

docker rm postgres --force

docker rm mongodb --force

kubectl delete -f ./eck/eck.yaml
```