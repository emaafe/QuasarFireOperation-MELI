# QuasarFireOperation-MELI
****URL Servicio****: https://quasarfireoperation-meli.azurewebsites.net
****Docs****: 
  * Postman Collection
  * Database SQL
  * Pruebas Funcionales

En el archivo "Pruebas Funcionales" se puede observar como actúan los controladores en diversos casos.

El archivo "Database SQL" encuentran la creación de las tablas SQL utilizadas para grabar información del servicio.

En el archivo "Postman Collection" les facilita agregar los endpoint a la aplicación Postman para realizar las pruebas.

****Endpoints**** ({satellite_name} reemplazar con el nombre del satélite):
  * https://quasarfireoperation-meli.azurewebsites.net/topsecret
  * https://quasarfireoperation-meli.azurewebsites.net/topsecret_split?satellite_name={satellite_name}
  * https://quasarfireoperation-meli.azurewebsites.net/topsecret_split/{satellite_name}/

****JSON Modelos****
  * ****topsecret****:
    ``{
      "satellites": [
        {
          "name": "kenobi",
          "distance": 100,
          "message": [
            "este",
            "",
            "",
            "mensaje",
            ""
          ]
        },
        {
          "name": "skywalker",
          "distance": 115.5,
          "message": [
            "",
            "es",
            "",
            "",
            "secreto"
          ]
        },
        {
          "name": "sato",
          "distance": 142.7,
          "message": [
            "este",
            "",
            "un",
            "",
            ""
          ]
        }
      ]
    }``
    
  * ****topsecret_split****:
    ``{
      "distance": 115.5,
        "message": [
          "",
          "es",
          "",
          "",
          "secreto"
        ]
    }``
