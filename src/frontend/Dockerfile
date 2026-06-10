# Etapa 1: Compilación de la SPA
FROM node:22-alpine AS build-step
WORKDIR /app

# Copiar manifiestos e instalar dependencias
COPY package*.json ./
RUN npm install

# Copiar código fuente y compilar para producción
COPY . .
RUN npm run build -- --configuration=production

# Etapa 2: Servidor Nginx para los estáticos
FROM nginx:alpine
# Angular por defecto compila en dist/[nombre-proyecto]/browser
COPY --from=build-step /app/dist/frontend/browser /usr/share/nginx/html

# Exponer puerto de la web
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]