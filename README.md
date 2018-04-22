# **a** Golden Shoe experiment

## About
This is an experiment.

## Ports used:
- backend-host (api gateway): 80:8080
- authenticator-service: 9000
- products-service: 9001
- orders-service: 9002
- tracking-service: 9003
- database: 3307:3306
- graphiql: 4000
- adminer: 8082:8080
- varnish: 8081:80
- zookeeper: 2181
- kafka: 9092

## Installation guide:
1. run backend/messages/build.cmd
2. run docker-compose up
3. navigate to 127.0.0.1:8082, connect to hostname "database" with username "root" and password "<empty>", selecte database products, and execute the SQL commands found in database-import/products.sql
4. cd into frontend/ and execute `npm i && npm run dev`
5. navigate to 127.0.0.1:3000

## Contact:

https://ioanb.com