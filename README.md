# GST Tools

Índice de las aplicaciones para proyectos de líneas de GESTAMP con el estándar SICAR@GST y TIA Portal. Cada aplicación tiene su propio repositorio, con su código, su documentación y sus versiones; aquí solo se explica para qué sirve cada una y dónde descargarla.

Si te son útiles, puedes invitarme a un café:

[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-ffdd00?logo=buymeacoffee&logoColor=black)](https://buymeacoffee.com/nsuarez75)

| Aplicación | Para qué sirve | Descargar |
| --- | --- | --- |
| [GST-LoS-Generator](https://github.com/nsuarez75/GST-LoS-Generator) | A partir de la LoS (Excel de señales I4.0), genera el FB `User_LoS_Device_In_FB` (XML de Openness) y un UDT `GSDML_DeviceNN` por device. | [Última versión](https://github.com/nsuarez75/GST-LoS-Generator/releases/latest) |
| [GST-FIT-Generator](https://github.com/nsuarez75/GST-FIT-Generator) | Se definen en la app las features de cada referencia y genera la plantilla FIT, el Excel FIT para la plataforma I4 y el código de PLC de trazabilidad (`User_TableInfo_DB`, `User_Config_Reference_FB`, `CheckTechTemplate_DB`). | [Última versión](https://github.com/nsuarez75/GST-FIT-Generator/releases/latest) |
| [ImportExportTIA](https://github.com/nsuarez75/ImportExportTIA) | Exporta bloques de un proyecto de TIA Portal V18/V19 a XML y los importa de vuelta, en el mismo proyecto o en otro, con TIA Openness. | [Última versión](https://github.com/nsuarez75/ImportExportTIA/releases/latest) |
| [HerramientasSicar](https://github.com/nsuarez75/HerramientasSicar) | Tareas sobre un proyecto SICAR@GST V4 abierto en TIA: DiagExpected, numerar mensajes, comentar pasos de secuencia en tres idiomas, renumerar arrays y generar la configuración del HMI. | [Última versión](https://github.com/nsuarez75/HerramientasSicar/releases/latest) |

## Cómo encajan

1. **Señales I4.0**: con la LoS de la línea, GST-LoS-Generator genera el FB de devices y sus UDT.
2. **Trazabilidad**: en GST-FIT-Generator se dan de alta las features de la pieza; genera el FIT para la plataforma I4 y los bloques de trazabilidad del PLC.
3. **Llevarlo a TIA**: el XML del FB de GST-LoS-Generator se importa con ImportExportTIA. Las fuentes `.db`, `.awl` y `.udt` se añaden en TIA como fuentes externas y los `.scl` de GST-FIT-Generator se copian en un bloque SCL.
4. **Ajustes del proyecto**: con el proyecto abierto en TIA, HerramientasSicar hace el resto de tareas del estándar (mensajes, comentarios, HMI…).

## Requisitos

| Aplicación | Sistema | Necesita |
| --- | --- | --- |
| GST-LoS-Generator | Windows o Linux, 64 bits | Nada: ejecutable único. |
| GST-FIT-Generator | Windows o Linux, 64 bits | Nada: ejecutable único. |
| ImportExportTIA | Windows | .NET Framework 4.8, TIA Portal V18 o V19 con Openness y el usuario en el grupo «Siemens TIA Openness». |
| HerramientasSicar | Windows | .NET Framework 4.8, TIA Portal con Openness y el usuario en el grupo «Siemens TIA Openness». |

Las aplicaciones con la interfaz nueva (GST-LoS-Generator, GST-FIT-Generator e ImportExportTIA) comparten aspecto, están en castellano e inglés y avisan al arrancar si hay una versión nueva.

## Mantener este índice

Los enlaces de descarga apuntan siempre a la última versión de cada aplicación, así que no hay que tocarlos al publicar una release. Solo hay que actualizar este README al añadir o retirar una aplicación, o si cambia para qué sirve. `./checks.sh` comprueba que todos los repositorios enlazados existen.
