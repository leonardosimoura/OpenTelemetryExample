
# Install ECK 

kubectl create -f https://download.elastic.co/downloads/eck/2.1.0/crds.yaml


kubectl apply -f https://download.elastic.co/downloads/eck/2.1.0/operator.yaml


kubectl apply -f ./eck.yaml


# Configs


## Kibana 

http://localhost:5601/

user elastic
senha:
    abrir o dashboad e procurar o secret "quickstart-es-elastic-user"


## APM Integration

add apm integration

quickstart-apm-http:8200