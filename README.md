<div align="center">

# Clasesópolis  
### Sistema Multimedia Mínimo Viable (SMMV)

Videojuego educativo 3D con estética **cyberpunk** para la enseñanza de Programación Orientada a Objetos (POO).

</div>

## Descripción
**Clasesópolis** es un videojuego educativo en 3D con estética *cyberpunk*, diseñado para facilitar la enseñanza y el aprendizaje de los conceptos abstractos de la Programación Orientada a Objetos (POO). A través de un entorno interactivo y gamificado, los estudiantes resuelven retos lógicos y estructurales guiados por el "Inspector Byte".

Este proyecto fue desarrollado por el equipo **GrouPOO** como proyecto final para la asignatura Diseño Multimedia 2 (Ingeniería Multimedia - Universidad Autónoma de Occidente).

## Características Principales (Alcance de los Sprints 1 al 4)
* **Sistema de Autenticación y Persistencia:** Integración en la nube para guardar el progreso del jugador, experiencia (XP), rachas e insignias.
* **Tutorial Interactivo:** Flujo de *onboarding* inmersivo que introduce las mecánicas de juego y la narrativa.
* **Fases de Aprendizaje de POO:**
  * **Fase 1:** Clases y Objetos (Mecánicas Drag & Drop).
  * **Fase 2 y 3:** Atributos, Tipos de Datos y Métodos.
  * **Fase 4:** Asociaciones estructurales (Composición y Agregación).
* **Modo Libre (Ciudad 3D):** Exploración del entorno *cyberpunk* con mecánicas de vuelo e interacción con edificios.

## Tecnologías y Arquitectura
El proyecto fue construido bajo el patrón arquitectónico **Modelo-Vista-Controlador (MVC)** para garantizar escalabilidad, separación de responsabilidades y mantenibilidad del código.

* **Motor Gráfico:** Unity 3D
* **Lenguaje de Programación:** C#
* **Base de Datos / Backend:** Firebase (Authentication & Firestore)
* **Control de Versiones:** Git & GitHub (con Git LFS para manejo de assets pesados)
* **Gestión de Interfaz:** Unity UI (Canvas, TextMeshPro)

## Instrucciones Básicas de Ejecución
Para clonar y ejecutar este proyecto en un entorno local, sigue estos pasos:

1. **Clonar el repositorio:**
   Asegúrate de tener instalado [Git LFS](https://git-lfs.github.com/).
   ```bash
   git clone [https://github.com/](https://github.com/)[TU-USUARIO]/clasesopolis.git

2. Abrir en Unity:
* Abre Unity Hub y selecciona Add / Open.
* Navega hasta la carpeta clonada y selecciónala.
*Nota: Asegúrate de utilizar la versión de Unity especificada en los Project Settings (Recomendada: 2022.3 LTS o superior).*

3. Configuración de Firebase:
* Verifica que el archivo google-services.json esté correctamente ubicado en la carpeta Assets/StreamingAssets (o según la configuración de tu entorno).

4. Ejecución:
* Abre la escena MainMenu o Splash ubicada en Assets/Scenes/.
* Presiona el botón de Play en el editor.

**Equipo de Desarrollo (GrouPOO)**
* Miguel Mosquera: Scrum Master | QA y Documentación
* Daniel Cárdenas: Diseño UI/UX | Assets 3D
* Kevin Aristizábal: Dev Unity C# | Audio y Arquitectura de Datos
* Santiago Osorio: Dev Unity C# | Mecánicas Lógicas
* Juan José Gómez: Dev Unity C# | Animaciones e Interactividad
  
*Documentación generada para la entrega de Lanzamiento Final - UAO 2026.*

