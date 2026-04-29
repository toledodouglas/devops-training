import type { FormEvent } from 'react'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import * as categoriesApi from '../api/categoriesApi'
import * as productsApi from '../api/productsApi'
import type { CategoryDto, ProductDto } from '../api/types'
import { useAuth } from '../context/AuthContext'

export function ProductsPage() {
  const { logout } = useAuth()
  const navigate = useNavigate()

  const [categories, setCategories] = useState<CategoryDto[]>([])
  const [categoryFilterId, setCategoryFilterId] = useState<number>(0)

  const [products, setProducts] = useState<ProductDto[]>([])
  const [loadingProducts, setLoadingProducts] = useState(false)

  const [formNome, setFormNome] = useState('')
  const [formQuantidade, setFormQuantidade] = useState(0)
  const [formCategoryId, setFormCategoryId] = useState<number>(0)

  const [feedback, setFeedback] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false
    ;(async () => {
      try {
        const list = await categoriesApi.fetchCategories()
        if (cancelled) return
        setCategories(list)
        setFormCategoryId((prev) => (prev !== 0 ? prev : list[0]?.id ?? 0))
      } catch (e) {
        if (cancelled) return
        const msg =
          e instanceof Error
            ? e.message
            : 'Não foi possível carregar categorias.'
        setFeedback(msg)
      }
    })()
    return () => {
      cancelled = true
    }
  }, [])

  const loadProducts = useCallback(async () => {
    setLoadingProducts(true)
    setFeedback(null)
    try {
      const list = await productsApi.fetchProducts(
        categoryFilterId > 0 ? categoryFilterId : undefined
      )
      setProducts(list)
    } catch (e) {
      const msg =
        e instanceof Error
          ? e.message
          : 'Não foi possível carregar os produtos.'
      setFeedback(msg)
    } finally {
      setLoadingProducts(false)
    }
  }, [categoryFilterId])

  useEffect(() => {
    void loadProducts()
  }, [loadProducts])

  const categoriesById = useMemo(() => {
    const map = new Map<number, string>()
    for (const c of categories) map.set(c.id, c.nome)
    return map
  }, [categories])

  async function onCreate(e: FormEvent) {
    e.preventDefault()
    setFeedback(null)
    try {
      if (formCategoryId <= 0) {
        throw new Error('Selecione uma categoria válida.')
      }
      await productsApi.createProduct({
        nome: formNome.trim(),
        quantidade: formQuantidade,
        categoryId: formCategoryId,
      })
      setFormNome('')
      setFormQuantidade(0)
      await loadProducts()
    } catch (err) {
      const msg =
        err instanceof Error ? err.message : 'Erro ao criar produto.'
      setFeedback(msg)
    }
  }

  async function onDelete(product: ProductDto) {
    if (!window.confirm(`Excluir ${product.nome}?`)) return
    setFeedback(null)
    try {
      await productsApi.deleteProduct(product.id)
      await loadProducts()
    } catch (err) {
      const msg =
        err instanceof Error ? err.message : 'Erro ao excluir produto.'
      setFeedback(msg)
    }
  }

  async function patchQuantity(product: ProductDto) {
    const raw = window.prompt(
      `Nova quantidade para ${product.nome}:`,
      String(product.quantidade)
    )
    if (raw === null) return
    const next = Number(raw)
    if (!Number.isFinite(next) || next < 0) {
      window.alert('Quantidade inválida.')
      return
    }
    setFeedback(null)
    try {
      await productsApi.updateProduct(product.id, {
        nome: product.nome,
        quantidade: next,
        categoryId: product.categoryId,
      })
      await loadProducts()
    } catch (err) {
      const msg =
        err instanceof Error ? err.message : 'Erro ao atualizar produto.'
      setFeedback(msg)
    }
  }

  async function onDecrease(product: ProductDto) {
    const raw = window.prompt('Quantidade a remover do estoque:', '1')
    if (raw === null) return
    const q = Number(raw)
    if (!Number.isFinite(q) || q <= 0) {
      window.alert('Informe uma quantidade maior que zero.')
      return
    }
    setFeedback(null)
    try {
      await productsApi.decreaseStock(product.id, Math.floor(q))
      await loadProducts()
    } catch (err) {
      const msg =
        err instanceof Error ? err.message : 'Erro ao baixar estoque.'
      setFeedback(msg)
    }
  }

  return (
    <div className="shell">
      <header className="topbar">
        <div className="topbar-brand">
          <strong>Estoque</strong>
          <span className="muted-inline">Painel</span>
        </div>
        <div className="topbar-actions">
          <button className="ghost" type="button" onClick={() => loadProducts()}>
            Atualizar
          </button>
          <button
            className="ghost"
            type="button"
            onClick={() => {
              logout()
              navigate('/login', { replace: true })
            }}
          >
            Sair
          </button>
        </div>
      </header>

      <main className="surface">
        {feedback ? <p className="banner-error">{feedback}</p> : null}

        <section className="panel">
          <div className="panel-header">
            <h2>Produtos</h2>
          </div>

          <div className="toolbar">
            <label className="toolbar-label">
              Filtrar categoria:
              <select
                value={categoryFilterId}
                onChange={(e) =>
                  setCategoryFilterId(Number.parseInt(e.target.value, 10))
                }
              >
                <option value={0}>Todas</option>
                {categories.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.nome}
                  </option>
                ))}
              </select>
            </label>
            <span className="muted-inline">
              {loadingProducts ? 'Carregando…' : `${products.length} itens`}
            </span>
          </div>

          <div className="table-wrap">
            <table className="table">
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>Categoria</th>
                  <th>Qtd</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                {products.map((p) => (
                  <tr key={p.id}>
                    <td>{p.nome}</td>
                    <td>
                      {p.categoriaNome ??
                        categoriesById.get(p.categoryId) ??
                        `#${p.categoryId}`}
                    </td>
                    <td>{p.quantidade}</td>
                    <td className="row-actions">
                      <button
                        type="button"
                        className="mini"
                        onClick={() => patchQuantity(p)}
                      >
                        Editar qtd
                      </button>
                      <button
                        type="button"
                        className="mini"
                        onClick={() => onDecrease(p)}
                      >
                        Baixar
                      </button>
                      <button
                        type="button"
                        className="mini danger"
                        onClick={() => onDelete(p)}
                      >
                        Excluir
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>

        <section className="panel">
          <div className="panel-header">
            <h2>Novo produto</h2>
          </div>

          <form className="form-grid" onSubmit={onCreate}>
            <label>
              Nome
              <input
                required
                value={formNome}
                onChange={(e) => setFormNome(e.target.value)}
              />
            </label>

            <label>
              Quantidade inicial
              <input
                type="number"
                min={0}
                required
                value={Number.isNaN(formQuantidade) ? 0 : formQuantidade}
                onChange={(e) =>
                  setFormQuantidade(Number.parseInt(e.target.value, 10))
                }
              />
            </label>

            <label>
              Categoria
              <select
                required
                value={formCategoryId}
                onChange={(e) =>
                  setFormCategoryId(Number.parseInt(e.target.value, 10))
                }
              >
                <option value={0}>Selecione</option>
                {categories.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.nome}
                  </option>
                ))}
              </select>
            </label>

            <div className="form-actions">
              <button type="submit" className="primary">
                Adicionar
              </button>
            </div>
          </form>
        </section>
      </main>
    </div>
  )
}
