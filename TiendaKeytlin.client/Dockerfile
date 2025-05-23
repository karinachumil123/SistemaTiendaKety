# Etapa de build
FROM node:18-alpine as build
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build -- --configuration production

# Etapa de producción con Nginx
FROM nginx:alpine
COPY --from=build /app/dist/tiendakeytlin.client /usr/share/nginx/html
EXPOSE 80
