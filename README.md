# Sistema de Controle de Afastamento de Professores

## Documentação do domínio:
- [Definição de Requisitos](./Docs/SCAP_DocumentoDefinicaoRequisitos.pdf)
- [Especificação de Requisitos](./Docs/SCAP_DocumentoEspecificacaoRequisitos.pdf)

## Requisitos de ambiente:
- .NET Core 3.1
- MySQL 5.6


## Configurações extras:
As variáveis de ambiente abaixo precisam ser definidas nos arquivos `appSettings.*.json` ou `docker-compose.yml` para que a conexão com o banco de dados possa ser estabelecida e os serviços de **E-mail** e **Logging** funcionem adequadamente:

```json
"ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=scap_db_dev;uid=root;password=;allow user variables=true"
},
"EmailSender": {
	"Host": "smtp.mailtrap.io",
	"Port": "2525",
	"EnableSSL": "True",
	"FromEmail": "no-reply@scap.dev",
	"UserName": "USER_NAME",
	"Password": "PASSWORD"
},
"Kisslog": {
	"OrganizationId": "ORGANIZATION_ID",
	"ApplicationId": "APPLICATION_ID",
	"ApiUrl": "https://api.kisslog.net"
}
```

Para executar a aplicação, tenha uma instância do MySQL em funcionamento e utilize o seguinte comando:
```bash
dotnet run
```