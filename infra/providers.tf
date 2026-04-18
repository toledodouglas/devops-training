#infra/providers.tf

terraform{
    required_version = ">= 1.5.0"

    required_providers{
        azurerm = {
            source = "hashicorp/azurerm"
            version = "~> 3.116.0"
        }
    }

    backend "azurerm" {
       resource_group_name  = "rg-terraform-state"
       storage_account_name = "sttfstatetoledo"
       container_name       = "tfstate"
       key                  = "dev.terraform.tfstate"
    }
}

provider "azurerm" {
    features{}
}