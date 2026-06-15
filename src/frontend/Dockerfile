# Stage 1: SPA Compilation
FROM node:22-alpine AS build-step
WORKDIR /app

# Copy manifests and install dependencies
COPY package*.json ./
RUN npm install

# Copy source code and compile for production
COPY . .
RUN npm run build -- --configuration=production

# Stage 2: Nginx server for static files
FROM nginx:alpine
# Angular compiles by default to dist/[project-name]/browser
COPY --from=build-step /app/dist/frontend/browser /usr/share/nginx/html

# Expose port of the web
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]