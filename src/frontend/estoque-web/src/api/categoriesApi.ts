import { apiRequest } from './client'
import type { CategoryDto } from './types'

export function fetchCategories() {
  return apiRequest<CategoryDto[]>('/Categories')
}
