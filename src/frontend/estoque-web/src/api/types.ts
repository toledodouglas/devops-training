export type LoginResponse = {
  accessToken: string
}

export type UserDto = {
  id: string
  nomeCompleto: string
  idade: number
  email: string
}

export type LoginRequest = {
  email: string
  senha: string
}

export type RegisterRequest = {
  nomeCompleto: string
  idade: number
  email: string
  senha: string
}

export type CategoryDto = {
  id: number
  nome: string
}

export type ProductDto = {
  id: number
  nome: string
  quantidade: number
  categoryId: number
  categoriaNome?: string | null
}
