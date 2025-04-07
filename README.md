# ExSales

Sistema de Vendas com Doações usando Marketing de Rede e Marketing Multinível.

## Descrição Geral

O ExSales é uma plataforma que permite o gerenciamento de doações através de representantes organizados em redes, com um modelo de marketing multinível. O sistema possui diferentes perfis de usuários e funcionalidades específicas para doadores, representantes, administradores de rede e o administrador master.

---

## Funcionalidades

### 1. Tela de Doação

- Cada representante terá um link personalizado para a tela de doação, armazenando automaticamente quem é o representante indicado;
- O usuário poderá escolher entre planos de doação preestabelecidos ou criar um plano personalizado (valor e parcelas);
- Alguns planos poderão ser acessados apenas por links específicos de representantes;
- O pagamento padrão será por cartão de crédito;
- Caso o usuário não esteja logado, será redirecionado para a tela de cadastro com opção de login;
- Campos obrigatórios no cadastro:
  - Nome completo
  - E-mail
  - CPF
- Campos opcionais:
  - Telefone
- O cadastro será salvo mesmo sem a finalização da doação;
- Um e-mail de validação será enviado automaticamente;
- O usuário estará com o status “Não Validado” até confirmar o e-mail;
- Usuários “Não Validados” ainda podem realizar doações;
- Não é permitido mais de um cadastro com o mesmo e-mail;
- Após login ou cadastro, o usuário prossegue para a tela de pagamento;
- Pagamentos imediatos (como cartão) confirmam a doação instantaneamente e enviam e-mail de agradecimento;
- Pagamentos pendentes (como PIX ou boleto) geram o comprovante na tela e por e-mail;
- Um processo em background verifica e atualiza pagamentos periodicamente.

---

### 2. Cadastro de Representante

- Representantes compartilham o mesmo cadastro de usuários comuns, com um tipo diferenciado;
- Podem ser cadastrados por indicação de outro representante, com registro da indicação;
- Campos obrigatórios no cadastro:
  - Nome completo
  - E-mail
  - Telefone
  - CPF
  - Data de nascimento
  - Endereço completo
  - Upload de documento
  - Dados bancários
- Após o cadastro, o status inicial será “Aguardando Aprovação”;
- Representantes não podem acessar o sistema até serem aprovados;
- E-mails são enviados em cada etapa (cadastro, aprovação, bloqueio);
- Representantes bloqueados não podem acessar o sistema nem receber comissões.

---

### 3. Administração

#### 3.1. Perfil Doador

- Acesso ao extrato de doações com:
  - Data de vencimento
  - Nome da rede
  - Valor (sem comissão)
  - Situação (Aberto, Pago ou Pendente)
  - Botão “Pagar” para situações "Aberto" ou "Pendente"
- Possibilidade de selecionar entre as redes que participa.

#### 3.2. Perfil Representante

- Links:
  - Link de doação e link de indicação de novos representantes
- Opção de saque:
  - Visualização de saldo disponível e próxima data de saque
  - Botão "Sacar" (quando permitido)
  - Confirmação dos dados bancários e solicitação via sistema
  - E-mails de solicitação e confirmação de saque
- Lista de usuários indicados:
  - Com filtros por nome, tipo e status
  - Campos: Nome, E-mail, Telefone, Tipo, Status, Extrato
- Extrato de doações:
  - Apenas valores pagos
  - Agrupado por rede
  - Campos: Data de vencimento, Valor, Comissão, Situação

#### 3.3. Perfil Administrador de Rede

- Cadastro e gerenciamento da própria rede:
  - Nome da rede
  - Porcentagem da rede (padrão 0)
  - Templates de e-mails
  - E-mail da rede
  - Valor mínimo e período mínimo de saque
- Acesso a todos os usuários da rede
- Extrato da rede:
  - Agrupado por representante
  - Com informações de data, valor, comissão e situação

#### 3.4. Perfil Administrador Master

- Acesso total a todas as redes
- Lista de redes cadastradas:
  - Nome, E-mail, Situação (Ativo/Inativo)
- Cadastro de redes:
  - Mesmos campos do administrador de rede, com opção de ativar/inativar
- Pode alternar entre redes no menu superior

---

## Tecnologias

* .NET Core
* React
* Next.js
* Postgres

---

## Como Contribuir

1. Faça um fork do projeto.
2. Crie uma branch com a sua feature: `git checkout -b minha-feature`.
3. Commit suas alterações: `git commit -m 'Minha nova feature'`.
4. Faça push para a branch: `git push origin minha-feature`.
5. Abra um Pull Request.

---

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

---

## Contato
