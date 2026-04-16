resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location

  tags = {
    projeto    = "devops-training"
    ambiente   = var.environment
    managed_by = "terraform"
  }
}

# Service Plan - Infraestrutura de computação (máquina virtual - Hardware)
resource "azurerm_service_plan" "plan" {
  name                = "sp-devops-training-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = var.app_service_sku
}

resource "azurerm_linux_web_app" "app" {
  name                = "app-devops-training-${var.environment}-toledo" 
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_service_plan.plan.location
  service_plan_id     = azurerm_service_plan.plan.id

  client_certificate_enabled = false

  site_config {
    application_stack {
      dotnet_version = "8.0" 
    }
    always_on = false 
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = var.environment == "prod" ? "Production" : "Development"
  }
}