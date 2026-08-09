# 📘 Flower E-Commerce Microservices - CI/CD Pipeline Guidebook

Welcome to the official **CI/CD Pipeline Guidebook** for the Flower E-Commerce Microservices project. This document provides an end-to-end reference explaining how our automated build, verification, and deployment pipeline works, how it was created step-by-step, and how to configure all required credentials.

---

## 📑 Table of Contents
1. [Chapter 1: Overview & Architecture](#chapter-1-overview--architecture)
2. [Chapter 2: Step-by-Step Pipeline Creation](#chapter-2-step-by-step-pipeline-creation)
3. [Chapter 3: Prerequisites & Secrets Setup (Docker Hub & GitHub)](#chapter-3-prerequisites--secrets-setup-docker-hub--github)
4. [Chapter 4: Deep Dive into the CI/CD Workflow File](#chapter-4-deep-dive-into-the-cicd-workflow-file)
5. [Chapter 5: Troubleshooting & Best Practices](#chapter-5-troubleshooting--best-practices)

---

## Chapter 1: Overview & Architecture

### Purpose
The CI/CD (Continuous Integration / Continuous Deployment) pipeline automates code quality checks, container image compilation, and artifact publishing. Every time code is pushed to the `main` or `master` branch (or submitted via a Pull Request), GitHub Actions automatically:
- Builds container images for all 7 microservices in parallel.
- Verifies that every service compiles without build errors.
- Pushes the resulting Docker images directly to **Docker Hub** upon successful push to production branches.

### Solution Microservices Map
The pipeline manages container builds for all 7 components in the repository:

| Service Name | Dockerfile Location | Docker Hub Image Name |
| :--- | :--- | :--- |
| **API Gateway** | `./API Gateway/Dockerfile` | `flower-apigateway` |
| **Identity Service** | `./Services/Identity/Identity.Api/Dockerfile` | `flower-identity-api` |
| **Catalog Service** | `./Services/Catalog/Catalog Service/Dockerfile` | `flower-catalog-service` |
| **Cart Service** | `./Services/Cart/Cart Service/Dockerfile` | `flower-cart-service` |
| **Order & Fulfillment Service** | `./Services/Order & Fulfillment/Order & Fulfillment Service/Dockerfile` | `flower-order-service` |
| **Payment Service** | `./Services/Payment/Payment Service/Dockerfile` | `flower-payment-service` |
| **Address & Store Coverage** | `./Services/Address & Store Coverage/Address & Store Coverage Service/Dockerfile` | `flower-address-service` |

---

## Chapter 2: Step-by-Step Pipeline Creation

Building a robust CI/CD workflow for a multi-project microservices repository required solving several key design requirements:

### Step 1: Standardizing Build Contexts
- **Problem**: Visual Studio generates default Dockerfiles assuming single-project restores. In multi-project solutions (e.g. `Identity.Api` referencing `BuildingBlocks` and `Identity.Application`), builds from the repository root failed due to missing `.csproj` paths.
- **Solution**: Updated relative `COPY` instructions in each `Dockerfile` so that context is anchored at the repo root (`.`), enabling proper restoration of internal project dependencies.

### Step 2: Designing Matrix Builds for Parallel Execution
- Rather than building 7 images sequentially (which would take ~10–15 minutes), we utilized a GitHub Actions `matrix` strategy.
- Set `fail-fast: false` so that if one microservice build fails, GitHub Actions continues building the rest of the services, giving developers complete visibility into all failing/passing services.

### Step 3: Handling Case-Sensitivity in Container Registries
- **Problem**: OCI (Open Container Initiative) & Docker Hub enforce that all image tags and repository names must be strictly **lowercase**. If a GitHub username contains uppercase characters (e.g., `TeamUser`), Docker rejects image tagging.
- **Solution**: Added a dynamic `tr '[:upper:]' '[:lower:]'` bash step in the pipeline to sanitize the username before generating image tags.

### Step 4: Configuring Multi-Branch Triggers
- Configured the workflow trigger `on.push.branches` and `on.pull_request.branches` to support both `main` and `master` branch naming conventions.

---

## Chapter 3: Prerequisites & Secrets Setup (Docker Hub & GitHub)

To enable automatic image pushing to Docker Hub, you must configure two repository secrets in GitHub: `DOCKER_USERNAME` and `DOCKER_PASSWORD`.

### Step 1: Create a Docker Hub Account
1. Open your browser and navigate to [https://hub.docker.com](https://hub.docker.com).
2. Click **Sign Up** (or **Log In** if you already have an account).
3. Choose a **Docker ID** (this will be your `DOCKER_USERNAME`).
4. Complete the registration process and verify your email address.

---

### Step 2: Generate a Personal Access Token (PAT) on Docker Hub
> [!IMPORTANT]
> Using a Personal Access Token (PAT) instead of your raw account password is a security best practice. If a token is ever compromised, it can be revoked instantly without changing your primary account credentials.

1. Log in to [hub.docker.com](https://hub.docker.com).
2. Click on your profile icon in the top-right corner and select **Account Settings**.
3. In the left navigation menu, click **Personal Access Tokens**.
4. Click **Generate New Token**.
5. Fill in the token details:
   - **Access Token Description**: `GitHub Actions CI CD`
   - **Access Permissions**: Select `Read, Write, Delete` (or `Read & Write`).
6. Click **Generate**.
7. **Copy the generated token string immediately** (you will not be able to view it again!).

---

### Step 3: Add Secrets to Your GitHub Repository
1. Open your repository on GitHub (`https://github.com/elevate-flower-ecommerce-projects/Flower_ECommerce_Microservices-team3`).
2. Click on **Settings** (in the top navigation bar of the repository).
3. In the left sidebar, expand **Secrets and variables** and click **Actions**.
4. Click **New repository secret**.
5. Add the first secret:
   - **Name**: `DOCKER_USERNAME`
   - **Secret**: Enter your Docker Hub username (e.g., `johnsmith`).
   - Click **Add secret**.
6. Add the second secret:
   - **Name**: `DOCKER_PASSWORD`
   - **Secret**: Paste the Personal Access Token (PAT) you copied from Docker Hub.
   - Click **Add secret**.

Now your GitHub repository is securely connected to Docker Hub!

---

## Chapter 4: Deep Dive into the CI/CD Workflow File

The CI/CD workflow configuration is saved in [.github/workflows/ci-cd.yml](file:///d:/partition%20h/Elevate/flower%20ecommerce/team3/Flower_ECommerce_Microservices-team3/.github/workflows/ci-cd.yml). Below is an annotated breakdown of its components:

```yaml
name: CI/CD Docker Pipeline

# 1. Triggers: Run on pushes and PRs to main or master
on:
  push:
    branches: [ "main", "master" ]
  pull_request:
    branches: [ "main", "master" ]

jobs:
  build-and-push:
    runs-on: ubuntu-latest
    strategy:
      fail-fast: false # 2. Ensures all microservices finish building even if one fails
      matrix:
        include:
          - service_name: apigateway
            dockerfile: "API Gateway/Dockerfile"
            image_name: flower-apigateway
          - service_name: identity-api
            dockerfile: "Services/Identity/Identity.Api/Dockerfile"
            image_name: flower-identity-api
          - service_name: catalog-service
            dockerfile: "Services/Catalog/Catalog Service/Dockerfile"
            image_name: flower-catalog-service
          - service_name: cart-service
            dockerfile: "Services/Cart/Cart Service/Dockerfile"
            image_name: flower-cart-service
          - service_name: order-service
            dockerfile: "Services/Order & Fulfillment/Order & Fulfillment Service/Dockerfile"
            image_name: flower-order-service
          - service_name: payment-service
            dockerfile: "Services/Payment/Payment Service/Dockerfile"
            image_name: flower-payment-service
          - service_name: address-service
            dockerfile: "Services/Address & Store Coverage/Address & Store Coverage Service/Dockerfile"
            image_name: flower-address-service

    steps:
      # Step 1: Pull source code into the runner
      - name: Checkout Code
        uses: actions/checkout@v4

      # Step 2: Enable Docker Buildx for enhanced container compilation
      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3

      # Step 3: Convert Docker Username to lowercase for OCI compliance
      - name: Prepare Docker Username (Lowercase)
        id: prep
        run: |
          DOCKER_USER=$(echo "${{ secrets.DOCKER_USERNAME }}" | tr '[:upper:]' '[:lower:]')
          echo "username=${DOCKER_USER}" >> $GITHUB_OUTPUT

      # Step 4: Authenticate with Docker Hub using repository secrets
      - name: Log in to Docker Hub
        if: github.event_name != 'pull_request'
        uses: docker/login-action@v3
        with:
          username: ${{ secrets.DOCKER_USERNAME }}
          password: ${{ secrets.DOCKER_PASSWORD }}

      # Step 5: Build image & push to Docker Hub (push enabled only on main/master branch)
      - name: Build and Push Docker Image
        uses: docker/build-push-action@v6
        with:
          context: .
          file: ${{ matrix.dockerfile }}
          push: ${{ github.event_name == 'push' && (github.ref == 'refs/heads/main' || github.ref == 'refs/heads/master') }}
          provenance: false
          tags: |
            ${{ steps.prep.outputs.username }}/${{ matrix.image_name }}:latest
            ${{ steps.prep.outputs.username }}/${{ matrix.image_name }}:${{ github.sha }}
```

---

## Chapter 5: Troubleshooting & Best Practices

### Common Issues & How to Resolve Them

#### 1. Error: `invalid reference format: repository name must be lowercase`
- **Cause**: Docker image tags contain uppercase characters.
- **Fix**: Our workflow automatically converts your username to lowercase (`steps.prep.outputs.username`). Ensure image names in the matrix strategy use only lowercase letters and hyphens (e.g. `flower-catalog-service`).

#### 2. Error: `unauthorized: incorrect username or password`
- **Cause**: The GitHub repository secrets `DOCKER_USERNAME` or `DOCKER_PASSWORD` are missing or invalid.
- **Fix**: Verify your Docker Hub credentials and update the secrets in **Repository Settings > Secrets and variables > Actions**.

#### 3. Error: `Cannot find path /src/...` during `dotnet restore`
- **Cause**: Dockerfile relies on dependent `.csproj` files that were not copied prior to restore.
- **Fix**: Ensure all project references are copied in the `build` stage of the Dockerfile prior to executing `RUN dotnet restore`.

---

## Summary Checklist
- [x] Docker Hub account registered at [hub.docker.com](https://hub.docker.com).
- [x] Personal Access Token (PAT) created.
- [x] `DOCKER_USERNAME` and `DOCKER_PASSWORD` secrets added to GitHub repository.
- [x] Workflow file [.github/workflows/ci-cd.yml](file:///d:/partition%20h/Elevate/flower%20ecommerce/team3/Flower_ECommerce_Microservices-team3/.github/workflows/ci-cd.yml) committed and pushed to `main`/`master`.
