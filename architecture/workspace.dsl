workspace {

    model {
        user = person "Usuário" {
            description "Usuário que acessa o sistema para gerenciar entregas, produtos e fornecedores."
        }

        supplier_delivery_system = softwareSystem "Supplier Delivery System" {
            description "Sistema de gerenciamento de entregas, produtos e fornecedores, composto por microserviços."
        
            frontend = container "Frontend Web" {
                description "Interface web desenvolvida em React.js, utilizada pelos usuários para interagir com o sistema."
                technology "React.js"
            }

            auth = container "Auth Microservice" {
                description "Microserviço ASP.NET Core responsável por autenticação, registro de usuários e validação de tokens JWT."
                technology "ASP.NET Core"

                auth_controller = component "Controller" {
                    description "Recebe requisições HTTP, processa entradas e retorna respostas."
                    technology "ASP.NET Core Controller"
                }

                service = component "Service" {
                    description "Contém a lógica de negócio relacionada à autenticação e usuários."
                    technology "C# Service"
                }

                repository = component "Repository" {
                    description "Responsável pela persistência e recuperação de dados de usuários."
                    technology "Entity Framework Core"
                }

                auth_controller -> service "Invoca métodos de negócio"
                service -> repository "Persiste e recupera dados"
            }

            supplier = container "Supplier Microservice" {
                description "Microserviço ASP.NET Core responsável pelo gerenciamento de fornecedores, produtos e entregas."
                technology "ASP.NET Core"

                supplier_controller = component "Controller" {
                    description "Recebe requisições HTTP relacionadas a fornecedores, produtos e entregas."
                    technology "ASP.NET Core Controller"
                }

                supplier_service = component "Supplier Service" {
                    description "Lógica de negócio para gerenciamento de fornecedores."
                    technology "C# Service"
                }

                product_service = component "Product Service" {
                    description "Lógica de negócio para gerenciamento de produtos."
                    technology "C# Service"
                }

                delivery_service = component "Delivery Service" {
                    description "Lógica de negócio para gerenciamento de entregas."
                    technology "C# Service"
                }

                supplier_repository = component "Supplier Repository" {
                    description "Persistência e recuperação de dados de fornecedores."
                    technology "Entity Framework Core"
                }

                product_repository = component "Product Repository" {
                    description "Persistência e recuperação de dados de produtos."
                    technology "Entity Framework Core"
                }

                delivery_repository = component "Delivery Repository" {
                    description "Persistência e recuperação de dados de entregas."
                    technology "Entity Framework Core"
                }

                supplier_controller -> supplier_service "Invoca operações de fornecedores"
                supplier_controller -> product_service "Invoca operações de produtos"
                supplier_controller -> delivery_service "Invoca operações de entregas"

                supplier_service -> supplier_repository "Persiste e recupera dados de fornecedores"
                product_service -> product_repository "Persiste e recupera dados de produtos"
                delivery_service -> delivery_repository "Persiste e recupera dados de entregas"
            }

            db_auth = container "Auth Database" {
                description "Banco de dados do microserviço de autenticação."
                technology "SQL Server"
                tags "Database"
            }

            db_supplier = container "Supplier Database" {
                description "Banco de dados do microserviço de fornecedores, produtos e entregas."
                technology "SQL Server"
                tags "Database"
            }

            repository -> db_auth "Acessa dados de usuários"
            supplier_repository -> db_supplier "Acessa dados de fornecedores, produtos e entregas"
            product_repository -> db_supplier "Acessa dados de fornecedores, produtos e entregas"
            delivery_repository -> db_supplier "Acessa dados de fornecedores, produtos e entregas"
        }

        user -> frontend "Utiliza via navegador"
        frontend -> auth_controller "Realiza login, registro e validação de token via API REST"
        frontend -> supplier_controller "Gerencia fornecedores, produtos e entregas via API REST"
        supplier_controller -> auth_controller "Valida JWT para autenticação" "HTTP (JWT)"

    }

    views {
        systemContext supplier_delivery_system {
            include *
            autolayout lr
            title "Contexto do Sistema Supplier Delivery"
        }

        container supplier_delivery_system {
            include *
            autolayout lr
            title "Visão de Contêineres do Sistema Supplier Delivery"
        }

        component auth {
            include *
            autolayout lr
            title "Visão de Componentes do Auth Microservice"
        }

        component supplier {
            include *
            autolayout lr
            title "Visão de Componentes do Supplier Microservice"
        }

        styles {
            element "Database" {
                shape cylinder
                background #f0f0f0
                color #333333
            }
        }

        theme default
    }
}