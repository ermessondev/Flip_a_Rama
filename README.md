# Script de Movimentação 2D no Unity

Este script implementa as principais mecânicas de movimentação de um personagem 2D no Unity, incluindo **movimento horizontal**, **pulo duplo**, **dash** e interação com **plataformas atravessáveis**.

---

## ✨ Funcionalidades

- **Movimento lateral**  
  Controlado pelo Input System (`OnMove`).  
  O personagem anda para a esquerda/direita de acordo com o valor recebido.

- **Pulo e Pulo Duplo**  
  - `OnJump()` permite pular se o personagem estiver no chão ou sobre uma plataforma.  
  - Caso esteja no ar e ainda não tenha usado o pulo duplo, pode executar um segundo salto.  
  - O dash recarrega o pulo duplo.

- **Dash (Impulso Rápido)**  
  - Ativado por `OnDash()`.  
  - Desliga a gravidade temporariamente, aplicando uma força lateral.  
  - Duração: `0.3s`  
  - Tempo de recarga: `2s`  

- **Plataformas Atravessáveis (One-Way)**  
  - O personagem pode subir pelas plataformas sem bater de baixo para cima.  
  - Ao apertar o comando de **cair** (`OnFallOfPlatform`), o personagem ignora a colisão da plataforma por `0.5s`, permitindo atravessar para baixo.  

---

## ⚙️ Configurações Necessárias no Unity

1. **Layers**  
   Crie e organize os seguintes layers no seu projeto:
   - `Ground` → Chão sólido.  
   - `Platform` → Plataformas atravessáveis.  
   - `Player` → Personagem jogador.  

   Certifique-se de atribuir corretamente cada GameObject ao seu respectivo layer.

2. **Objeto "pePlayer"**  
   - Dentro do Player, crie um GameObject filho chamado `pePlayer`.  
   - Ele será usado como ponto de referência para verificar contato com o chão/plataforma.  
   - Normalmente posicionado logo abaixo do sprite do personagem.

3. **Componentes Obrigatórios no Player**  
   O script exige estes componentes:  
   ```csharp
   // Caso o objeto não tenha o componente, ele é criado em tempo de compilação
   [RequireComponent(typeof(Rigidbody2D))]
   [RequireComponent(typeof(PlayerInput))]
   [RequireComponent(typeof(BoxCollider2D))]
