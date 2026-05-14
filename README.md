# Proyecto Bases de Datos: Juego en Unity

Este proyecto consiste en el desarrollo de un juego en Unity que hace uso de una base de datos relacional. El objetivo es integrar la lógica del juego con la persistencia de datos, siguiendo una serie de pasos que incluyen el diseño de la base de datos, la implementación del DDL y DML, y la programación de la aplicación en Unity para interactuar con dicha base de datos.

## Miembros del Grupo

- Karin Ibarra Cano

## Diagrama Entidad-Relación (E-R)

![Diagrama E-R](https://private-us-east-1.manuscdn.com/sessionFile/yLScq7pDnjja5cErt5TuoH/sandbox/nRIFeWixs7KdaaoXVVlfyL-images_1778754768361_na1fn_L2hvbWUvdWJ1bnR1L0RpYWdyYW1hRVI.png?Policy=eyJTdGF0ZW1lbnQiOlt7IlJlc291cmNlIjoiaHR0cHM6Ly9wcml2YXRlLXVzLWVhc3QtMS5tYW51c2Nkbi5jb20vc2Vzc2lvbkZpbGUveUxTY3E3cERuamphNWNFcnQ1VHVvSC9zYW5kYm94L25SSUZlV2l4czdLZGFhb1hWVmxmeUwtaW1hZ2VzXzE3Nzg3NTQ3NjgzNjFfbmExZm5fTDJodmJXVXZkV0oxYm5SMUwwUnBZV2R5WVcxaFJWSS5wbmciLCJDb25kaXRpb24iOnsiRGF0ZUxlc3NUaGFuIjp7IkFXUzpFcG9jaFRpbWUiOjE3OTg3NjE2MDB9fX1dfQ__&Key-Pair-Id=K2HSFNDJXOU9YS&Signature=AYRgmNhuBa2IWUuUxzAebvFBJDdEFdzi~OP~YFZuM~9skPKpHWrHoBg7pAZMNXKZ-rBnerl0lE7wJC9tdBAz936hEoRiUZQfUk3QN1MpkIXijK2FjYGG-ChUAkEvINoEm7REks594lG1kIZCUckvrJpcyXSoLVfjqeXYjypXNT76GrLzSTuaovVReeHICW2UTQnWDhZPZ43zpegSPRj3-MCkDB6-PJ3MJBqWkpisPoLZ3VK~qQ4hDYcwiixKmDRQjCdYVUubccb3YT0TBBJojypbY6EaTM3mHnQ7LJ7YqbQpbJnO8TalQr04NdXNONnVXUYc2UibX3GpTT0LhWtxAw__)

### Comentario sobre el Diagrama E-R

El diagrama Entidad-Relación (E-R) modela la estructura de la base de datos para el juego. Las entidades principales son **Jugador**, **Partida**, **Tareas**, **Enemigo**, **QuickTimeEvents**, **PuertaDeSalida**, **EnemigoBasico**, **EnemigoDeRango**, **Proyectil** y **EnemigoDeAgarre**.

- **Jugador**: Representa al personaje controlado por el usuario con atributos como vida, estamina y velocidad.

- **Partida**: El espacio donde ocurre y se ejecutan todos los procesos y estados del juego.

- **Tareas**: Define los objetivos que el jugador debe completar, con atributos como duración y la probabilidad de activar un QTE.

- **Enemigo**: Es una entidad general para los diferentes tipos de enemigos, con atributos como daño, velocidad, memoria y rango de visión. EnemigoBásico, EnemigoDeRango y EnemigoDeAgarre.

- **QuickTimeEvents**: Eventos de tiempo rápido que pueden ocurrir y alertar a los enemigos se no se cumplen.

- **PuertaDeSalida**: Representa el punto de salida en el juego, asociado a una partida.

- **EnemigoBásico**: Un tipo de enemigo con un rango de ataque a mele.

- **EnemigoDeRango**: Un tipo de enemigo que dispara proyectiles, con distancia de disparo y visión adicional.

- **Proyectil**: Los proyectiles disparados por los enemigos de rango, con velocidad, tiempo de vida y daño.

- **EnemigoDeAgarre**: Un tipo de enemigo que puede estirar sus brazos para agarrar al jugador, con cooldown y rango de agarre.

Las relaciones clave incluyen:

- Un **Jugador** puede jugar muchas **Partidas** (1 a N).

- Un **Jugador** **Completa** muchas **Tareas** (N a M).

- Las **Tareas** pueden **Alertar **muchos **Enemigos** (M a N).

- Una **Partida** **Contiene** una **PuertaDeSalida** (1 a 1).

- Un **EnemigoDeRango** **Dispara** muchos **Proyectiles** (1 a N).

- Al completar 3 **Tareas** los **Enemigos** reciben un aumento de velocidad.

- Multiples **Enemigos**  atacan a un **Jugador.**



## Tablas de la Base de Datos Relacional

### Tabla: Jugador

| Columna | Tipo de Dato | Nulo | PK | FK |
| --- | --- | --- | --- | --- |
| id_jugador | INT | NO | YES | NO |
| vida | INT | NO | NO | NO |
| estamina | INT | NO | NO | NO |
| velocidad | DECIMAL(5,2) | NO | NO | NO |
| velocidad_al_correr | DECIMAL(5,2) | NO | NO | NO |

### Tabla: Partida

| Columna | Tipo de Dato | Nulo | PK | FK |
| --- | --- | --- | --- | --- |
| id_partida | INT | NO | YES | NO |
| estado | VARCHAR(50) | NO | NO | NO |
| id_jugador | INT | NO | NO | YES |

### Tabla: Tareas

| Columna | Tipo de Dato | Nulo | PK | FK |
| --- | --- | --- | --- | --- |
| id_tarea | INT | NO | YES | NO |
| duracion | INT | NO | NO | NO |
| probabilidad_de_QTE | DECIMAL(5,2) | NO | NO | NO |
| velocidad | DECIMAL(5,2) | NO | NO | NO |

### Tabla: QuickTimeEvents

| Columna | Tipo de Dato | Nulo | PK | FK |
| --- | --- | --- | --- | --- |
| id_qte | INT | NO | YES | NO |
| descripcion | VARCHAR(255) | YES | NO | NO |

### Tabla: Enemigo

| Columna | Tipo de Dato | Nulo | PK | FK |
| --- | --- | --- | --- | --- |
| id_enemigo | INT | NO | YES | NO |
| daño | INT | NO | NO | NO |
| velocidad | DECIMAL(5,2) | NO | NO | NO |
| memoria | INT | NO | NO | NO |
| rango_vision | DECIMAL(5,2) | NO | NO | NO |
| estado | VARCHAR(50) | NO | NO | NO |
| id_qte | INT | YES | NO | YES |

### Tabla: PuertaDeSalida

| Columna | Tipo de Dato | Nulo | PK | FK |
| --- | --- | --- | --- | --- |
| id_puerta | INT | NO | YES | NO |
| ubicacion | VARCHAR(255) | YES | NO | NO |
| id_partida | INT | NO | NO | YES |

### Tabla: EnemigoBasico

| Columna | Tipo de Dato | Nulo | PK | FK |
| --- | --- | --- | --- | --- |
| id_enemigo | INT | NO | YES | YES |
| rango_ataque | DECIMAL(5,2) | NO | NO | NO |

### Tabla: EnemigoDeRango

| Columna | Tipo de Dato | Nulo | PK | FK |
| --- | --- | --- | --- | --- |
| id_enemigo | INT | NO | YES | YES |
| distancia_disparo | DECIMAL(5,2) | NO | NO | NO |
| vision_cubierta_adicional | DECIMAL(5,2) | NO | NO | NO |

### Tabla: Proyectil

| Columna | Tipo de Dato | Nulo | PK | FK |
| --- | --- | --- | --- | --- |
| id_proyectil | INT | NO | YES | NO |
| velocidad | DECIMAL(5,2) | NO | NO | NO |
| tiempo_de_vida | INT | NO | NO | NO |
| daño | INT | NO | NO | NO |
| id_enemigo_de_rango | INT | NO | NO | YES |

### Tabla: EnemigoDeAgarre

| Columna | Tipo de Dato | Nulo | PK | FK |
| --- | --- | --- | --- | --- |
| id_enemigo | INT | NO | YES | YES |
| cooldown_agarre | INT | NO | NO | NO |
| rango_agarre | DECIMAL(5,2) | NO | NO | NO |

### Tabla: Jugador_Completa_Tareas

| Columna | Tipo de Dato | Nulo | PK | FK |
| --- | --- | --- | --- | --- |
| id_jugador | INT | NO | YES | YES |
| id_tarea | INT | NO | YES | YES |
| tiempo_completado | INT | YES | NO | NO |

### Tabla: Tareas_Alerta_Enemigo

| Columna | Tipo de Dato | Nulo | PK | FK |
| --- | --- | --- | --- | --- |
| id_tarea | INT | NO | YES | YES |
| id_enemigo | INT | NO | YES | YES |
| condicion_alerta | VARCHAR(255) | YES | NO | NO |

### Tabla: Enemigo_Ataca_Jugador

| Columna | Tipo de Dato | Nulo | PK | FK |
| --- | --- | --- | --- | --- |
| id_enemigo | INT | NO | YES | YES |
| id_jugador | INT | NO | YES | YES |
| tiempo_ataque | INT | YES | NO | NO |

