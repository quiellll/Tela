# Práctica I - Tela

# Animación 3D. Bloque II

> Ezequiel García Díaz - 3º curso en Grado en Diseño y Desarrollo de Videojuegos.
> 

## Introducción

La realización de esta primera práctica consiste en la simulación de las deformaciones de una tela en Unity implementando las físicas de forma manual. La práctica se divide en 3 requisitos, con otros adicionales de mejoras.

---

## Instrucciones de ejecución.

Para ejecutar la simulación desarrollada, es importante importar el paquete de Unity adjunto a un proyecto vacío, habiendo eliminado previamente cualquier archivo creado automáticamente por Unity.

> 💡 Para importar el paquete, navegar a "Assets" → "Import Package" → "Custom Package…" y seleccionar el archivo "Tela_EzequielGarcíaDíaz.unitypackage"
>



Una vez importados los archivos, se recomienda cargar por la escena *FixedCloth,* ubicada en la carpeta “*Scenes*”. Esta escena muestra una simulación básica de la tela. Con presionar el botón *Play* (▶️) de Unity, la simulación dará comienzo.

Para modificar los parámetros de la simulación, basta con seleccionar el *GameObject* “*Cloth*” y editar los parámetros del componente *MassSpringCloth* en el inspector. Los valores por defecto deberían proporcionar una simulación satisfactoria y realista.

Se puede interactuar con la simulación si se cambia la ventana de Unity de modo *Game* a *Scene*, ganando control sobre los *GameObjects* de la escena. Se puede seleccionar el *Fixer* que sujeta la tela desplegando el *GameObject “Cloth”,* y seleccionando el *GameObject* “*Fixer*” posteriormente. Mover el *Fixer* durante la ejecución de la simulación dotará de movimiento a la tela asociada al *Fixer*

### Escenas Adicionales

Con motivo de mostrar la funcionalidad completa de la simulación desarrollada, se han creado varias escenas adicionales que hacen uso de distintos recursos implementados.

Estas escenas están disponibles en la carpeta *Assets/Scenes* del proyecto.

### Escena *FixedClothMultipleFix*

Esta escena muestra la posibilidad de vincular más de un *Fixer* a una tela, dotándola de varios puntos de anclaje. También proporciona valores de simulación diferentes para mostrar otro comportamiento distinguible del caso base.

### Escena *Flag*

En esta escena se representa una simplificación de una bandera. El objetivo de esta escena es demostrar el funcionamiento del viento como factor integrado en la simulación. Por defecto los valores del viento están activados y configurados, pero se pueden editar mediante el Inspector en el componente *MassSpringCloth* del *GameObject* “*Cloth*”.

## Requisito 1. Componente MassSpringCloth

Se pide crear el componente *MassSpringCloth* para realizar una simulación básica de la tela. De las propiedades solicitadas, se cumplen:

- Componente *MassSpringCloth* añadible a un *GameObject* con componente *Mesh*.
- Inicialización de nodos mediante vértices de la malla.
- Inicialización de muelles mediante aristas de la malla.
- Gestión de aristas evitando duplicados.
- Integración simpléctica de Euler.
- *GameObject* transformable.

---

## Requisito 2. Componente fixer

Se pide crear un componente que permita fijar nodos del objeto *MassSpringCloth*. Se ha implementado mediante un componente *Fixer* añadible a otro *GameObject*, de forma que los nodos de la tela que entren en contacto con el *Fixer* queden fijados.

- Componente *Fixer* añadible a un *GameObject* con componente *Collider.*
- *Fixer* transformable que modifica la posición del *GameObject MassSpringCloth*.


>💡 Para vincular un *Fixer* a un *MassSpringCloth*, se debe colocar el componente *Fixer* como hijo del *GameObject* *MassSpringCloth* en la jerarquía de escena.
>


Esta implementación permite que una tela pueda verse afectada por más de un *Fixer*, mejorando la simulación (Demostrado en la escena “*FixedClothMultiple*”).

---

## Requisito 3. Muelles de tracción y flexión

En este apartado, se proponen varios problemas. Nos encontramos frente a la creación de varios tipos de muelle; la eliminación de muelles repetidos; y agregación de muelles de flexión (considerando positivamente un algoritmo eficiente). Se han cumplido todos los requisitos:

- Diferenciación entre tipos de muelle. Manejables desde el inspector con parámetros editables.
- Eliminación de muelles repetidos. Abarcado en el requisito 1.
- Implementación de muelles de flexión.
- Algoritmo eficiente para muelles de flexión. Implementado mediante el uso de un *HashSet.*

---

## Requisitos Adicionales.

Finalmente, se solicita la implementación de varias funcionalidades adicionales para mejorar y completar la simulación. De las mejoras propuestas, se han implementado las siguientes:

- Simulación con *substeps*.
- Amortiguamiento. Modelos de amortiguamiento aplicado en nodos y en muelles.
- Viento. Aplicación de fuerzas externas a la tela como simulación de viento
- Aspectos visuales. Texturizado de las telas.
- Aspectos de interacción. Posibilidad de modificar los valores de la simulación en el inspector incluso durante la ejecución de la simulación.
- Implementación de *Prefabs*
