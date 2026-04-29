import { apiRequest } from './client'
import type { ProductDto } from './types'

export function fetchProducts(categoryId?: number) {
  if (categoryId && categoryId > 0) {
    return apiRequest<ProductDto[]>(`/Products/by-category/${categoryId}`)
  }

  return apiRequest<ProductDto[]>('/Products')
}

export type ProductCreateRequest = {
  nome: string
  quantidade: number
  categoryId: number
}

export type ProductUpdateRequest = ProductCreateRequest

export function createProduct(payload: ProductCreateRequest) {
  return apiRequest<ProductDto>('/Products', {
    method: 'POST',
    json: {
      nome: payload.nome,
      quantidade: payload.quantidade,
      categoryId: payload.categoryId,
    },
  })
}

export function updateProduct(id: number, payload: ProductUpdateRequest) {
  return apiRequest<ProductDto>(`/Products/${id}`, {
    method: 'PUT',
    json: {
      nome: payload.nome,
      quantidade: payload.quantidade,
      categoryId: payload.categoryId,
    },
  })
}

export function deleteProduct(id: number) {
  return apiRequest<void>(`/Products/${id}`, { method: 'DELETE' })
}

export function decreaseStock(id: number, quantidadeRemovida: number) {
  return apiRequest<ProductDto>(`/Products/${id}/decrease-stock`, {
    method: 'PATCH',
    json: { quantidadeRemovida },
  })
}
