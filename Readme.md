# Trabalho de PWA

**Professor:** Camillo Falcão  
**Instituição:** Faculdades Integradas Vianna Junior  
**Alunos:** 
- Matheus Zacanini
- Fernando Zuchi
- Arthur Reis
- Vitor Augusto

## Sales Funnel - PWA Master

### Resumo do Projeto

Este projeto consiste em um funil de vendas para um produto digital chamado "PWA Master", desenvolvido em dotnet blazor wasm e tailwind css. O objetivo é capturar leads interessados em desenvolvimento de PWAs, oferecendo conteúdo gratuito para engajamento e convertendo-os em clientes de um e-book/curso completo sobre o tema.

### Fluxo do Funil

1. **Landing Page** (`LandingPage.razor`)
   - Ponto de entrada do usuário
   - Apresentação inicial do produto/serviço
   - Call-to-action para direcionar ao formulário de cadastro
   - Exibição de benefícios e diferenciais para atrair o usuário

2. **Formulário de Cadastro** (`FormularioCadastro.razor`)
   - Captura de dados do lead (nome, email)
   - Apresentação de benefícios exclusivos
   - Após submissão, exibe mensagem de sucesso
   - Direciona o usuário para a página de conteúdo de engajamento

3. **Conteúdo de Engajamento** (`ConteudoEngajamento.razor`)
   - Disponibiliza conteúdo gratuito de valor para o usuário
   - Estruturado em diferentes seções:
     - Conteúdo em destaque
     - Materiais gratuitos
     - Conteúdos essenciais
     - Preview de conteúdo premium
   - Inclui CTAs estratégicos para direcionar à página de oferta
   - Componentes interativos para aumentar o engajamento do usuário

4. **Página de Oferta** (`PaginaOferta.razor`)
   - Apresentação detalhada do produto comercial
   - Tabela de preços com diferentes opções (básico e premium)
   - Depoimentos e provas sociais para aumentar credibilidade
   - Detalhamento do conteúdo oferecido
   - Botões de compra que finalizariam o funil

### Tecnologias Utilizadas

- Blazor WebAssembly
- Tailwind CSS
- Componentes reutilizáveis

Este projeto demonstra a implementação de um funil de vendas completo usando tecnologias modernas de desenvolvimento web, utilizando Blazor wasm e TailWind conforme visto em sala de aula.