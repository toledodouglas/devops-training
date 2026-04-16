variable "resource_group_name" {
  description = "Nome do grupo de recursos"
  type        = string
}

variable "location" {
  description = "Região da Azure onde os recursos serão criados"
  type        = string
  default     = "East US"
}

variable "environment" {
  description = "Ambiente (dev, hom, prod)"
  type        = string
}

variable "app_service_sku" {
  description = "Tamanho da máquina para o App Service"
  type        = string
}