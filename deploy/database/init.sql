CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


-- ### PRODUTO ###
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240119154908_BaseInicialProdutos') THEN
    CREATE TABLE "Produtos" (
        "Id" uuid NOT NULL,
        "Nome" varchar(100) NOT NULL,
        "ValorUnitario" money NOT NULL,
        "Ativo" bool NOT NULL,
        "Categoria" integer NOT NULL,
        "TempoPreparoEstimado" integer NOT NULL,
        "Descricao" text NOT NULL,
        CONSTRAINT "PK_Produtos" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240119154908_BaseInicialProdutos') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240119154908_BaseInicialProdutos', '8.0.0');
    END IF;
END $EF$;
COMMIT;


-- ### CUPOM ###
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240120121326_BaseInicialCupons') THEN
    CREATE TABLE "Cupons" (
        "Id" uuid NOT NULL,
        "DataInicio" timestamp with time zone NOT NULL,
        "DataFim" timestamp with time zone NOT NULL,
        "CodigoCupom" character varying(10) NOT NULL,
        "PorcentagemDesconto" numeric NOT NULL,
        "Status" integer NOT NULL,
        CONSTRAINT "PK_Cupons" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240120121326_BaseInicialCupons') THEN
    CREATE TABLE "CupomProdutos" (
        "Id" uuid NOT NULL,
        "CupomId" uuid NOT NULL,
        "ProdutoId" uuid NOT NULL,
        CONSTRAINT "PK_CupomProdutos" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_CupomProdutos_Cupons_CupomId" FOREIGN KEY ("CupomId") REFERENCES "Cupons" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240120121326_BaseInicialCupons') THEN
    CREATE INDEX "IX_CupomProdutos_CupomId" ON "CupomProdutos" ("CupomId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240120121326_BaseInicialCupons') THEN
    CREATE INDEX "IX_Cupons_CodigoCupom" ON "Cupons" ("CodigoCupom");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240120121326_BaseInicialCupons') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240120121326_BaseInicialCupons', '8.0.0');
    END IF;
END $EF$;
COMMIT;


-- ### ESTOQUE ###
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240120123100_BaseInicialEstoques') THEN
    CREATE TABLE "Estoques" (
        "Id" uuid NOT NULL,
        "ProdutoId" uuid NOT NULL,
        "Quantidade" integer NOT NULL,
        CONSTRAINT "PK_Estoques" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240120123100_BaseInicialEstoques') THEN
    CREATE TABLE "MovimentacoesEstoque" (
        "Id" uuid NOT NULL,
        "ProdutoId" uuid NOT NULL,
        "Quantidade" integer NOT NULL,
        "TipoMovimentacao" integer NOT NULL,
        "OrigemMovimentacao" integer NOT NULL,
        "DataLancamento" timestamp with time zone NOT NULL,
        "EstoqueId" uuid NOT NULL,
        CONSTRAINT "PK_MovimentacoesEstoque" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_MovimentacoesEstoque_Estoques_EstoqueId" FOREIGN KEY ("EstoqueId") REFERENCES "Estoques" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240120123100_BaseInicialEstoques') THEN
    CREATE UNIQUE INDEX "IX_Estoques_ProdutoId" ON "Estoques" ("ProdutoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240120123100_BaseInicialEstoques') THEN
    CREATE INDEX "IX_MovimentacoesEstoque_EstoqueId" ON "MovimentacoesEstoque" ("EstoqueId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240120123100_BaseInicialEstoques') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240120123100_BaseInicialEstoques', '8.0.0');
    END IF;
END $EF$;
COMMIT;


-- ### CARRINHO ###
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240113153837_BaseInicialCarrinho') THEN
    CREATE TABLE "Carrinho" (
        "Id" uuid NOT NULL,
        "ClienteId" uuid,
        "ValorTotal" numeric NOT NULL,
        "TempoMedioPreparo" integer NOT NULL,
        "DataCriacao" timestamp with time zone NOT NULL,
        "DataAtualizacao" timestamp with time zone,
        CONSTRAINT "PK_Carrinho" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240113153837_BaseInicialCarrinho') THEN
    CREATE TABLE "ItensCarrinho" (
        "Id" uuid NOT NULL,
        "ValorUnitario" numeric NOT NULL,
        "Quantidade" integer NOT NULL,
        "ProdutoId" uuid NOT NULL,
        "Nome" text NOT NULL,
        "TempoEstimadoPreparo" integer NOT NULL,
        "CarrinhoId" uuid NOT NULL,
        CONSTRAINT "PK_ItensCarrinho" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ItensCarrinho_Carrinho_CarrinhoId" FOREIGN KEY ("CarrinhoId") REFERENCES "Carrinho" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240113153837_BaseInicialCarrinho') THEN
    CREATE INDEX "IDX_Cliente" ON "Carrinho" ("ClienteId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240113153837_BaseInicialCarrinho') THEN
    CREATE INDEX "IX_ItensCarrinho_CarrinhoId" ON "ItensCarrinho" ("CarrinhoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240113153837_BaseInicialCarrinho') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240113153837_BaseInicialCarrinho', '8.0.0');
    END IF;
END $EF$;
COMMIT;


-- ### PEDIDO ###
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240113134414_BaseInicialPedidos') THEN
    CREATE TABLE "Pedidos" (
        "Id" uuid NOT NULL,
        "ClienteId" uuid,
        "Cpf" varchar(11),
        "Status" integer NOT NULL,
        "ValorTotal" numeric NOT NULL,
        "CupomId" uuid,
        "DataCriacao" timestamp with time zone NOT NULL,
        "DataAtualizacao" timestamp with time zone,
        CONSTRAINT "PK_Pedidos" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240113134414_BaseInicialPedidos') THEN
    CREATE TABLE "ItensPedido" (
        "Id" uuid NOT NULL,
        "ValorUnitario" numeric NOT NULL,
        "Desconto" numeric,
        "ValorFinal" numeric NOT NULL,
        "Quantidade" integer NOT NULL,
        "ProdutoId" uuid NOT NULL,
        "PedidoId" uuid NOT NULL,
        CONSTRAINT "PK_ItensPedido" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ItensPedido_Pedidos_PedidoId" FOREIGN KEY ("PedidoId") REFERENCES "Pedidos" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240113134414_BaseInicialPedidos') THEN
    CREATE INDEX "IX_ItensPedido_PedidoId" ON "ItensPedido" ("PedidoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240113134414_BaseInicialPedidos') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240113134414_BaseInicialPedidos', '8.0.0');
    END IF;
END $EF$;
COMMIT;



/*
##########
INSERÇÃO DE DADOS
##########
*/

-- ### PRODUTOS ###
INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('6e02169a-bf32-4e6b-bdd5-079e5eecdee0', 'X-Burger', 11.5, True, 0, 10, 'Pão, hambúrguer 150g, queijo');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('8868a194-ea4f-4cbe-be59-430bd725cfe1', 'X-Catupiry', 19, True, 0, 10,
        'Pão, hambúrguer 150g, catupiry, alface, tomate');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('4c0b8687-0ffc-4628-9157-565ee3cb3986', 'X-Salada', 14, True, 0, 10,
        'Pão, hambúrguer 150g, queijo, alface, tomate');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('869f38db-cd29-40d1-bfc1-d5f933a3f53e', 'X-Egg', 18, True, 0, 10,
        'Pão, hambúrguer 150g, queijo, ovo, alface, tomate');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('7681f2e5-00df-4bd8-be17-0df54291b978', 'X-Milho', 18, True, 0, 10,
        'Pão, hambúrguer 150g, queijo, milho, alface, tomate');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('b6511523-73d8-4496-9c82-80f3567ae328', 'X-Americano', 21.50, True, 0, 10,
        'Pão, hambúrguer 150g, queijo, ovo, milho, alface, tomate');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('6f96ec3e-20ba-4842-b45d-e596ed812d3c', 'X-Bacon', 23, True, 0, 10,
        'Pão, hambúrguer 150g, queijo, bacon, alface, tomate');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('68552c00-853b-47b4-81aa-ad4eb92d9360', 'X-Calabresa', 23, True, 0, 10,
        'Pão, hambúrguer 150g, queijo, calabresa, alface, tomate');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('e5c14e37-8c26-4524-8445-a0f0c0650b6c', 'X-Tudo', 26.50, True, 0, 10,
        'Pão, hambúrguer 150g, queijo, bacon, ovo, milho, alface, tomate');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('45887e07-b7a8-4c16-baef-f722ef12fe16', 'X-EasyFood', 30, True, 0, 10,
        'Pão, 2 hambúrgueres 150g, queijo, bacon, ovo, milho, alface, tomate, molho especial');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('abd2a168-0115-4403-86bc-cd4937cabec2', 'Anéis de Cebola', 10, True, 1, 10, 'Anéis de Cebola');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('ff6117af-2775-45bb-9b50-21c229238822', 'Batata frita', 6, True, 1, 10, 'Batata frita');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('175f9ba8-179e-4cca-b105-10a94dff990e', 'Batata Com cheddar e bacon', 15, True, 1, 10,
        'Batata Com cheddar e bacon');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('1a803e69-f44d-4adf-b5a4-8673ba73a8e4', 'Calabresa acebolada', 22, True, 1, 10, 'Calabresa acebolada');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('0ef9dd4a-2da5-4ceb-bad0-026164d406dc', 'Cola-Cola lata 350ml', 6.5, True, 2, 2, 'Cola-Cola lata 350ml');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('30c07e56-d098-42ca-a777-42ff4cfd40dc', 'Cola-Cola zero lata 350ml', 6.5, True, 2, 2,
        'Cola-Cola zero lata 350ml');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('1603d200-3078-4708-8820-e65830165515', 'Fanta laranja lata 350ml', 6.5, True, 2, 2,
        'Fanta laranja lata 350ml');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('2c9677b3-fe61-4fc8-b130-91ca2bd043e3', 'Fanta uva lata 350ml', 6.5, True, 2, 2, 'Fanta uva lata 350ml');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('32a13b97-ffcd-4fcc-96bf-3f33e98ac37f', 'Guaraná Antarctica lata 350ml', 6.5, True, 2, 2,
        'Guaraná Antarctica lata 350ml');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('6bd4a2e0-bfc2-4607-a427-bbe5cfe7f829', 'Água sem gás', 5, True, 2, 2, 'Água sem gás');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('f37aed21-6c85-4d22-9b0c-7c398dd72db5', 'Água com gás', 5, True, 2, 2, 'Água com gás');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('4064bfaf-908e-409a-a7d0-2614ddaf8b9f', 'Suco de laranja natural(copo)', 8, True, 2, 5,
        'Suco de laranja natural(copo)');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('246458fd-9dba-4e94-bf57-c48b558663e7', 'Suco de maracujá natural(copo)', 8, True, 2, 5,
        'Suco de maracujá natural(copo)');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('545cc297-594a-43d9-ae2d-1f0305eacd72', 'Suco de morango natural(copo)', 8.5, True, 2, 5,
        'Suco de morango natural(copo)');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('8e329a2d-0255-4e74-ab5f-8116827a7a22', 'Milkshake de morango', 10, True, 3, 5, 'Milkshake de morango');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('d0d75c85-63cd-4a3f-910c-142db4451570', 'Milkshake de chocolate', 10, True, 3, 5, 'Milkshake de chocolate');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('bc23ae6c-8a7d-4578-9841-5064a2ee3f00', 'Milkshake de Óreo', 12, True, 3, 5, 'Milkshake de Óreo');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('7b5a598d-34f9-49a9-b519-d52a297a8ad4', 'Sorvete de creme', 5, True, 3, 5, 'Sorvete de creme');

INSERT INTO "Produtos"("Id", "Nome", "ValorUnitario", "Ativo", "Categoria", "TempoPreparoEstimado", "Descricao")
VALUES ('3cbcc258-005f-4663-85c6-e236e9c61d7b', 'Sorvete de chocolate', 5, True, 3, 5, 'Sorvete de chocolate');


-- ### ESTOQUE ###
INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('043e56ed-a284-4ea8-b6d7-2c0b2fdfa3cb', '6e02169a-bf32-4e6b-bdd5-079e5eecdee0', 20);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('576fecd7-5780-4727-a3a4-c69357d477bf', '6e02169a-bf32-4e6b-bdd5-079e5eecdee0', 20, 0, 0, current_timestamp,
        '043e56ed-a284-4ea8-b6d7-2c0b2fdfa3cb');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('5bf912b9-442f-4e54-8b1d-c19d1df0dec7', '8868a194-ea4f-4cbe-be59-430bd725cfe1', 22);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('69c18efb-1b20-404c-964f-6a9be1318d9a', '8868a194-ea4f-4cbe-be59-430bd725cfe1', 22, 0, 0, current_timestamp,
        '5bf912b9-442f-4e54-8b1d-c19d1df0dec7');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('c4322a0a-4b9d-4f05-9a1f-d8e0c09be35a', '4c0b8687-0ffc-4628-9157-565ee3cb3986', 25);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('7ecde999-7bff-4d0a-a569-63defee103e0', '4c0b8687-0ffc-4628-9157-565ee3cb3986', 25, 0, 0, current_timestamp,
        'c4322a0a-4b9d-4f05-9a1f-d8e0c09be35a');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('5784a76b-5ed4-42dc-947b-e4404d1cd706', '869f38db-cd29-40d1-bfc1-d5f933a3f53e', 30);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('c15150c7-d98f-430f-8657-7401f4842c53', '869f38db-cd29-40d1-bfc1-d5f933a3f53e', 30, 0, 0, current_timestamp,
        '5784a76b-5ed4-42dc-947b-e4404d1cd706');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('6905c7ae-7f9b-4a03-b4dd-604787d1b363', '7681f2e5-00df-4bd8-be17-0df54291b978', 10);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('6c2de906-b292-4efe-b715-b5fb7a5d9371', '7681f2e5-00df-4bd8-be17-0df54291b978', 10, 0, 0, current_timestamp,
        '6905c7ae-7f9b-4a03-b4dd-604787d1b363');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('f1723de3-b8aa-4412-a9d7-a507122878f0', 'b6511523-73d8-4496-9c82-80f3567ae328', 15);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('a24c44c1-098b-4bfc-b69a-b59dfba65ec3', 'b6511523-73d8-4496-9c82-80f3567ae328', 15, 0, 0, current_timestamp,
        'f1723de3-b8aa-4412-a9d7-a507122878f0');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('667f2437-6410-499c-89cb-d01216cfa1df', '6f96ec3e-20ba-4842-b45d-e596ed812d3c', 30);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('b6359a85-1f2b-451e-b7e5-5ec81db953b8', '6f96ec3e-20ba-4842-b45d-e596ed812d3c', 30, 0, 0, current_timestamp,
        '667f2437-6410-499c-89cb-d01216cfa1df');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('f7ada820-8469-4d35-b39e-b91ab6e4b53a', '68552c00-853b-47b4-81aa-ad4eb92d9360', 30);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('399635e1-7b99-4aca-a1fe-b39c49eb1585', '68552c00-853b-47b4-81aa-ad4eb92d9360', 30, 0, 0, current_timestamp,
        'f7ada820-8469-4d35-b39e-b91ab6e4b53a');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('40224e54-7911-44ec-b1e7-5ce72c4535bf', 'e5c14e37-8c26-4524-8445-a0f0c0650b6c', 40);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('6939d9bf-f8bd-4c07-836c-141a75d1ac87', 'e5c14e37-8c26-4524-8445-a0f0c0650b6c', 40, 0, 0, current_timestamp,
        '40224e54-7911-44ec-b1e7-5ce72c4535bf');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('cf641628-8d4c-43ff-a740-399914234a67', '45887e07-b7a8-4c16-baef-f722ef12fe16', 60);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('e6403be3-9153-407c-939d-f3ad63054d7c', '45887e07-b7a8-4c16-baef-f722ef12fe16', 60, 0, 0, current_timestamp,
        'cf641628-8d4c-43ff-a740-399914234a67');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('705d4e64-09a6-4a48-ad46-35ddc59fab4e', 'abd2a168-0115-4403-86bc-cd4937cabec2', 50);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('949d7fe8-372a-4855-b41c-a5f64aa97f80', 'abd2a168-0115-4403-86bc-cd4937cabec2', 50, 0, 0, current_timestamp,
        '705d4e64-09a6-4a48-ad46-35ddc59fab4e');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('a8d7a73c-077e-41b0-b136-7f3c8e463cce', 'ff6117af-2775-45bb-9b50-21c229238822', 100);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('912575d8-f30a-410c-a865-9835911c50d7', 'ff6117af-2775-45bb-9b50-21c229238822', 100, 0, 0, current_timestamp,
        'a8d7a73c-077e-41b0-b136-7f3c8e463cce');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('0e229b6e-0dfd-4eda-b148-ccec26ae9813', '175f9ba8-179e-4cca-b105-10a94dff990e', 80);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('aa4a4db3-1806-45fe-915b-62f09cf1eae1', '175f9ba8-179e-4cca-b105-10a94dff990e', 80, 0, 0, current_timestamp,
        '0e229b6e-0dfd-4eda-b148-ccec26ae9813');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('e8675bc3-b659-4fd4-bd66-3ff973b07935', '1a803e69-f44d-4adf-b5a4-8673ba73a8e4', 20);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('c2915a7f-7418-4701-a3e2-c342bec83404', '1a803e69-f44d-4adf-b5a4-8673ba73a8e4', 20, 0, 0, current_timestamp,
        'e8675bc3-b659-4fd4-bd66-3ff973b07935');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('344e108c-f8d1-4cb4-afc6-ea789d0b91cb', '0ef9dd4a-2da5-4ceb-bad0-026164d406dc', 30);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('3e8ed437-59db-48a4-b250-8268a1036348', '0ef9dd4a-2da5-4ceb-bad0-026164d406dc', 30, 0, 0, current_timestamp,
        '344e108c-f8d1-4cb4-afc6-ea789d0b91cb');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('4ca0fc28-ac89-4623-9d68-e66bbbb21b70', '30c07e56-d098-42ca-a777-42ff4cfd40dc', 35);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('27f7afc9-9c8b-47b2-8f78-fed4d7fa8fd6', '30c07e56-d098-42ca-a777-42ff4cfd40dc', 35, 0, 0, current_timestamp,
        '4ca0fc28-ac89-4623-9d68-e66bbbb21b70');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('94dea6e2-2d3f-4393-b67b-7e4ce2fa74a0', '1603d200-3078-4708-8820-e65830165515', 20);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('09b1f67a-3144-4cde-a30b-0c8700ffcdcb', '1603d200-3078-4708-8820-e65830165515', 20, 0, 0, current_timestamp,
        '94dea6e2-2d3f-4393-b67b-7e4ce2fa74a0');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('6b9e1f76-eae6-47bf-966c-5dbb88d7b1a0', '2c9677b3-fe61-4fc8-b130-91ca2bd043e3', 22);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('0500ac8f-5a47-46d4-a21f-a06dff1c7f14', '2c9677b3-fe61-4fc8-b130-91ca2bd043e3', 22, 0, 0, current_timestamp,
        '6b9e1f76-eae6-47bf-966c-5dbb88d7b1a0');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('352d10b2-d33f-4d64-9bde-9ccaa2bfa77b', '32a13b97-ffcd-4fcc-96bf-3f33e98ac37f', 40);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('0398cd30-86f2-4a1c-932b-a06b1b730cbf', '32a13b97-ffcd-4fcc-96bf-3f33e98ac37f', 40, 0, 0, current_timestamp,
        '352d10b2-d33f-4d64-9bde-9ccaa2bfa77b');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('ef12cd32-3175-4f26-aee4-e1b50301d121', '6bd4a2e0-bfc2-4607-a427-bbe5cfe7f829', 20);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('c645523b-2ba3-4888-9340-25c2377c888f', '6bd4a2e0-bfc2-4607-a427-bbe5cfe7f829', 20, 0, 0, current_timestamp,
        'ef12cd32-3175-4f26-aee4-e1b50301d121');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('1d0673a4-a1db-4946-bd50-d94046f0dd45', 'f37aed21-6c85-4d22-9b0c-7c398dd72db5', 25);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('64cf59f3-012c-4b93-b194-bf12bd97a318', 'f37aed21-6c85-4d22-9b0c-7c398dd72db5', 25, 0, 0, current_timestamp,
        '1d0673a4-a1db-4946-bd50-d94046f0dd45');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('2ba95ef2-1201-45c8-96ea-a93e9749a87c', '4064bfaf-908e-409a-a7d0-2614ddaf8b9f', 50);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('1d047695-8729-4432-8d88-a45b778ae680', '4064bfaf-908e-409a-a7d0-2614ddaf8b9f', 50, 0, 0, current_timestamp,
        '2ba95ef2-1201-45c8-96ea-a93e9749a87c');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('3afa2190-c327-4abe-aef3-09b457244a4e', '246458fd-9dba-4e94-bf57-c48b558663e7', 40);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('e9151def-2110-4e2d-a5e9-6f2546a14b1a', '246458fd-9dba-4e94-bf57-c48b558663e7', 40, 0, 0, current_timestamp,
        '3afa2190-c327-4abe-aef3-09b457244a4e');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('78e08249-a9e2-4d6a-9844-235c758531b4', '545cc297-594a-43d9-ae2d-1f0305eacd72', 35);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('2de051e6-81c2-4d94-bae3-e71806c40106', '545cc297-594a-43d9-ae2d-1f0305eacd72', 35, 0, 0, current_timestamp,
        '78e08249-a9e2-4d6a-9844-235c758531b4');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('1f9cf8e6-7b0b-479e-825c-b3a4e7453b5d', '8e329a2d-0255-4e74-ab5f-8116827a7a22', 20);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('620a278b-6613-408a-a37d-cbcfafb015f5', '8e329a2d-0255-4e74-ab5f-8116827a7a22', 20, 0, 0, current_timestamp,
        '1f9cf8e6-7b0b-479e-825c-b3a4e7453b5d');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('ac30c5dd-ee73-4838-b35e-e2c85b759252', 'd0d75c85-63cd-4a3f-910c-142db4451570', 20);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('5576e304-1057-4ce1-88c5-41cda2bab162', 'd0d75c85-63cd-4a3f-910c-142db4451570', 20, 0, 0, current_timestamp,
        'ac30c5dd-ee73-4838-b35e-e2c85b759252');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('e7112083-6d77-43e8-b431-e151c54d40c7', 'bc23ae6c-8a7d-4578-9841-5064a2ee3f00', 20);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('6aec1a55-bdde-49a4-8369-6a930eb4d6fd', 'bc23ae6c-8a7d-4578-9841-5064a2ee3f00', 20, 0, 0, current_timestamp,
        'e7112083-6d77-43e8-b431-e151c54d40c7');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('fa106947-b5bd-4d07-aaff-3f8d1ff6a9f9', '7b5a598d-34f9-49a9-b519-d52a297a8ad4', 20);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('89a1502b-9766-4a6e-a257-6397cc61ed09', '7b5a598d-34f9-49a9-b519-d52a297a8ad4', 20, 0, 0, current_timestamp,
        'fa106947-b5bd-4d07-aaff-3f8d1ff6a9f9');

INSERT INTO "Estoques"("Id", "ProdutoId", "Quantidade")
VALUES ('5456fee6-a334-40f2-a0b2-df33e9cabe63', '3cbcc258-005f-4663-85c6-e236e9c61d7b', 20);
INSERT INTO "MovimentacoesEstoque"("Id", "ProdutoId", "Quantidade", "TipoMovimentacao", "OrigemMovimentacao",
                                    "DataLancamento", "EstoqueId")
VALUES ('49dac569-f28f-4579-a068-22e9d9095e83', '3cbcc258-005f-4663-85c6-e236e9c61d7b', 20, 0, 0, current_timestamp,
        '5456fee6-a334-40f2-a0b2-df33e9cabe63');


-- ### CUPONS ###
INSERT INTO "Cupons"("Id", "DataInicio", "DataFim", "CodigoCupom", "PorcentagemDesconto", "Status")
VALUES ('bc8bcb70-da19-4e65-bc81-3020706f5944', '2024-01-28', '2024-02-07', 'easy10', 0.1, 1);
INSERT INTO "CupomProdutos"("Id", "CupomId", "ProdutoId")
VALUES ('5388422c-ac94-40b4-9732-d82161f45bb9', 'bc8bcb70-da19-4e65-bc81-3020706f5944',
        'e5c14e37-8c26-4524-8445-a0f0c0650b6c');
INSERT INTO "CupomProdutos"("Id", "CupomId", "ProdutoId")
VALUES ('ec9cad0a-1768-4e13-9fea-9892faf3f66a', 'bc8bcb70-da19-4e65-bc81-3020706f5944',
        '4c0b8687-0ffc-4628-9157-565ee3cb3986');
INSERT INTO "CupomProdutos"("Id", "CupomId", "ProdutoId")
VALUES ('81959f7c-8342-4beb-9249-6411f690d6dc', 'bc8bcb70-da19-4e65-bc81-3020706f5944',
        '869f38db-cd29-40d1-bfc1-d5f933a3f53e');

INSERT INTO "Cupons"("Id", "DataInicio", "DataFim", "CodigoCupom", "PorcentagemDesconto", "Status")
VALUES ('297e3c0d-860c-4a77-8fb6-fe87a83557a0', '2024-01-28', '2024-02-02', 'easy15', 0.15, 1);
INSERT INTO "CupomProdutos"("Id", "CupomId", "ProdutoId")
VALUES ('55caabe5-cb9d-4641-9f7b-d56bb5baa8d2', '297e3c0d-860c-4a77-8fb6-fe87a83557a0',
        'e5c14e37-8c26-4524-8445-a0f0c0650b6c');
INSERT INTO "CupomProdutos"("Id", "CupomId", "ProdutoId")
VALUES ('a37353a9-34c2-44f1-81ea-2cee2360b4dd', '297e3c0d-860c-4a77-8fb6-fe87a83557a0',
        '869f38db-cd29-40d1-bfc1-d5f933a3f53e');
INSERT INTO "CupomProdutos"("Id", "CupomId", "ProdutoId")
VALUES ('f0ad33e9-c85b-4116-bb93-338f687b3fbc', '297e3c0d-860c-4a77-8fb6-fe87a83557a0',
        '7681f2e5-00df-4bd8-be17-0df54291b978');
INSERT INTO "CupomProdutos"("Id", "CupomId", "ProdutoId")
VALUES ('035330f5-9400-47e8-a7c6-46b78a5b1994', '297e3c0d-860c-4a77-8fb6-fe87a83557a0',
        'b6511523-73d8-4496-9c82-80f3567ae328');
INSERT INTO "CupomProdutos"("Id", "CupomId", "ProdutoId")
VALUES ('7974e866-c666-46dc-a7b6-004a4996af8a', '297e3c0d-860c-4a77-8fb6-fe87a83557a0',
        '6e02169a-bf32-4e6b-bdd5-079e5eecdee0');
INSERT INTO "CupomProdutos"("Id", "CupomId", "ProdutoId")
VALUES ('75ef3da0-35b6-4ea7-9470-0a1328ed8e81', '297e3c0d-860c-4a77-8fb6-fe87a83557a0',
        '68552c00-853b-47b4-81aa-ad4eb92d9360');

INSERT INTO "Cupons"("Id", "DataInicio", "DataFim", "CodigoCupom", "PorcentagemDesconto", "Status")
VALUES ('a93b0335-3327-4f0a-b1b2-1bf0defacc56', '2024-02-02', '2024-02-17', 'easy20', 0.2, 1);
INSERT INTO "CupomProdutos"("Id", "CupomId", "ProdutoId")
VALUES ('2e458fd8-0d19-43fd-9f5d-aeb624929eda', 'a93b0335-3327-4f0a-b1b2-1bf0defacc56',
        '869f38db-cd29-40d1-bfc1-d5f933a3f53e');
INSERT INTO "CupomProdutos"("Id", "CupomId", "ProdutoId")
VALUES ('5dab716c-b958-453a-951b-ecfaa8fe0565', 'a93b0335-3327-4f0a-b1b2-1bf0defacc56',
        '7681f2e5-00df-4bd8-be17-0df54291b978');
INSERT INTO "CupomProdutos"("Id", "CupomId", "ProdutoId")
VALUES ('7b84b9ab-0ff8-4ceb-98bb-9611f01ee04c', 'a93b0335-3327-4f0a-b1b2-1bf0defacc56',
        '68552c00-853b-47b4-81aa-ad4eb92d9360');
INSERT INTO "CupomProdutos"("Id", "CupomId", "ProdutoId")
VALUES ('b28edc1c-3403-403c-9d77-2197967a0f18', 'a93b0335-3327-4f0a-b1b2-1bf0defacc56',
        '6e02169a-bf32-4e6b-bdd5-079e5eecdee0');


COMMIT;