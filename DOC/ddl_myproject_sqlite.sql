

PRAGMA foreign_keys = OFF;

DROP TABLE IF EXISTS evento_danio;
DROP TABLE IF EXISTS evento_qte;
DROP TABLE IF EXISTS partida_enemigo;
DROP TABLE IF EXISTS partida_tarea;
DROP TABLE IF EXISTS partida;
DROP TABLE IF EXISTS nivel_tarea;
DROP TABLE IF EXISTS tarea_config;
DROP TABLE IF EXISTS punto_patrulla;
DROP TABLE IF EXISTS nivel_enemigo;
DROP TABLE IF EXISTS punto_spawn;
DROP TABLE IF EXISTS enemigo_config;
DROP TABLE IF EXISTS proyectil_config;
DROP TABLE IF EXISTS tipo_enemigo;
DROP TABLE IF EXISTS puerta_salida;
DROP TABLE IF EXISTS configuracion_jugador;
DROP TABLE IF EXISTS nivel;
DROP TABLE IF EXISTS jugador;

-- =========================================================
-- 1. Catálogos y configuración base
-- =========================================================

CREATE TABLE jugador (
    id_jugador INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre_usuario TEXT NOT NULL UNIQUE,
    fecha_registro TEXT NOT NULL DEFAULT (STRFTIME('%Y-%m-%d %H:%M:%S', 'now')),
    activo INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE nivel (
    id_nivel INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    escena_unity TEXT NOT NULL UNIQUE,
    total_tareas_requeridas INTEGER NOT NULL DEFAULT 3,
    tareas_activas_por_partida INTEGER NOT NULL DEFAULT 3,
    multiplicador_velocidad_escape REAL NOT NULL DEFAULT 1.50,
    descripcion TEXT,
    activo INTEGER NOT NULL DEFAULT 1,
    creado_en TEXT NOT NULL DEFAULT (STRFTIME('%Y-%m-%d %H:%M:%S', 'now')),

    CHECK (total_tareas_requeridas > 0),
    CHECK (tareas_activas_por_partida > 0),
    CHECK (multiplicador_velocidad_escape >= 1.00)
);

CREATE TABLE configuracion_jugador (
    id_config_jugador INTEGER PRIMARY KEY AUTOINCREMENT,

    id_nivel INTEGER NOT NULL UNIQUE,

    velocidad_caminar REAL NOT NULL DEFAULT 5.00,
    velocidad_correr REAL NOT NULL DEFAULT 9.00,
    gravedad REAL NOT NULL DEFAULT -9.81,
    velocidad_giro REAL NOT NULL DEFAULT 10.00,
    stamina_max REAL NOT NULL DEFAULT 100.00,
    tasa_regeneracion_stamina REAL NOT NULL DEFAULT 20.00,
    tasa_consumo_stamina REAL NOT NULL DEFAULT 25.00,
    corazones_iniciales INTEGER NOT NULL DEFAULT 3,

    FOREIGN KEY (id_nivel)
        REFERENCES nivel(id_nivel)
        ON UPDATE CASCADE
        ON DELETE CASCADE,

    CHECK (stamina_max > 0),
    CHECK (corazones_iniciales > 0)
);

CREATE TABLE puerta_salida (
    id_puerta_salida INTEGER PRIMARY KEY AUTOINCREMENT,

    id_nivel INTEGER NOT NULL,

    nombre TEXT NOT NULL DEFAULT 'Door',
    offset_x REAL NOT NULL DEFAULT 3.000,
    offset_y REAL NOT NULL DEFAULT 0.000,
    offset_z REAL NOT NULL DEFAULT 0.000,
    velocidad_apertura REAL NOT NULL DEFAULT 2.00,

    posicion_x REAL,
    posicion_y REAL,
    posicion_z REAL,

    FOREIGN KEY (id_nivel)
        REFERENCES nivel(id_nivel)
        ON UPDATE CASCADE
        ON DELETE CASCADE,

    CHECK (velocidad_apertura > 0)
);

CREATE TABLE tipo_enemigo (
    id_tipo_enemigo INTEGER PRIMARY KEY AUTOINCREMENT,
    codigo TEXT NOT NULL UNIQUE,
    nombre TEXT NOT NULL,
    descripcion TEXT
);

CREATE TABLE proyectil_config (
    id_proyectil_config INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    prefab_unity TEXT NOT NULL UNIQUE,
    velocidad REAL NOT NULL DEFAULT 15.00,
    vida_segundos REAL NOT NULL DEFAULT 4.00,
    danio INTEGER NOT NULL DEFAULT 1,
    capa_unity TEXT DEFAULT 'Projectile',

    CHECK (velocidad > 0),
    CHECK (vida_segundos > 0),
    CHECK (danio > 0)
);

CREATE TABLE enemigo_config (
    id_enemigo_config INTEGER PRIMARY KEY AUTOINCREMENT,

    id_tipo_enemigo INTEGER NOT NULL,
    id_proyectil_config INTEGER,

    nombre TEXT NOT NULL,
    prefab_unity TEXT NOT NULL UNIQUE,

    velocidad_patrulla REAL NOT NULL DEFAULT 3.00,
    velocidad_persecucion REAL NOT NULL DEFAULT 6.00,
    distancia_vision REAL NOT NULL DEFAULT 10.00,
    angulo_vision REAL NOT NULL DEFAULT 90.00,
    radio_deteccion_cercana REAL NOT NULL DEFAULT 2.00,
    rango_ataque REAL NOT NULL DEFAULT 1.50,
    cooldown_ataque REAL NOT NULL DEFAULT 2.00,
    duracion_memoria REAL NOT NULL DEFAULT 5.00,

    distancia_preferida REAL,
    umbral_detencion REAL,
    windup_disparo REAL,
    angulo_dispersion REAL,
    cooldown_disparo REAL,

    cooldown_agarre REAL,
    windup_agarre REAL,
    rango_agarre REAL,
    fuerza_agarre REAL,
    distancia_segura REAL,
    velocidad_extension_brazo REAL,
    longitud_max_brazo REAL,
    radio_agarre_visual REAL,

    activo INTEGER NOT NULL DEFAULT 1,

    FOREIGN KEY (id_tipo_enemigo)
        REFERENCES tipo_enemigo(id_tipo_enemigo)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,

    FOREIGN KEY (id_proyectil_config)
        REFERENCES proyectil_config(id_proyectil_config)
        ON UPDATE CASCADE
        ON DELETE SET NULL,

    CHECK (velocidad_patrulla > 0 AND velocidad_persecucion > 0),
    CHECK (distancia_vision > 0 AND angulo_vision BETWEEN 0 AND 360),
    CHECK (rango_ataque > 0 AND cooldown_ataque >= 0)
);

-- =========================================================
-- 2. Elementos del nivel
-- =========================================================

CREATE TABLE punto_spawn (
    id_punto_spawn INTEGER PRIMARY KEY AUTOINCREMENT,

    id_nivel INTEGER NOT NULL,

    codigo TEXT NOT NULL,
    tipo_spawn TEXT NOT NULL CHECK (tipo_spawn IN ('JUGADOR', 'ENEMIGO')),

    posicion_x REAL NOT NULL,
    posicion_y REAL NOT NULL,
    posicion_z REAL NOT NULL,

    rotacion_y REAL NOT NULL DEFAULT 0.000,
    activo INTEGER NOT NULL DEFAULT 1,

    FOREIGN KEY (id_nivel)
        REFERENCES nivel(id_nivel)
        ON UPDATE CASCADE
        ON DELETE CASCADE,

    UNIQUE (id_nivel, codigo)
);

CREATE TABLE nivel_enemigo (
    id_nivel_enemigo INTEGER PRIMARY KEY AUTOINCREMENT,

    id_nivel INTEGER NOT NULL,
    id_enemigo_config INTEGER NOT NULL,

    cantidad_maxima INTEGER NOT NULL DEFAULT 1,
    seleccionable_spawn_aleatorio INTEGER NOT NULL DEFAULT 1,
    activo INTEGER NOT NULL DEFAULT 1,

    FOREIGN KEY (id_nivel)
        REFERENCES nivel(id_nivel)
        ON UPDATE CASCADE
        ON DELETE CASCADE,

    FOREIGN KEY (id_enemigo_config)
        REFERENCES enemigo_config(id_enemigo_config)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,

    UNIQUE (id_nivel, id_enemigo_config),

    CHECK (cantidad_maxima > 0)
);

CREATE TABLE punto_patrulla (
    id_punto_patrulla INTEGER PRIMARY KEY AUTOINCREMENT,

    id_nivel_enemigo INTEGER NOT NULL,

    orden INTEGER NOT NULL,

    posicion_x REAL NOT NULL,
    posicion_y REAL NOT NULL,
    posicion_z REAL NOT NULL,

    FOREIGN KEY (id_nivel_enemigo)
        REFERENCES nivel_enemigo(id_nivel_enemigo)
        ON UPDATE CASCADE
        ON DELETE CASCADE,

    UNIQUE (id_nivel_enemigo, orden)
);

CREATE TABLE tarea_config (
    id_tarea_config INTEGER PRIMARY KEY AUTOINCREMENT,

    nombre TEXT NOT NULL,
    prefab_unity TEXT NOT NULL UNIQUE,

    duracion_segundos REAL NOT NULL DEFAULT 6.00,
    qte_tiempo_min REAL NOT NULL DEFAULT 0.30,
    qte_tiempo_max REAL NOT NULL DEFAULT 0.70,
    qte_duracion REAL NOT NULL DEFAULT 2.00,
    velocidad_barra REAL NOT NULL DEFAULT 0.50,
    bonificacion_tiempo REAL NOT NULL DEFAULT 2.00,

    requiere_enemigo_asociado INTEGER NOT NULL DEFAULT 1,

    CHECK (duracion_segundos > 0),
    CHECK (qte_tiempo_min >= 0 AND qte_tiempo_max >= qte_tiempo_min),
    CHECK (qte_duracion > 0),
    CHECK (velocidad_barra > 0)
);

CREATE TABLE nivel_tarea (
    id_nivel_tarea INTEGER PRIMARY KEY AUTOINCREMENT,

    id_nivel INTEGER NOT NULL,
    id_tarea_config INTEGER NOT NULL,

    codigo TEXT NOT NULL,

    posicion_x REAL NOT NULL,
    posicion_y REAL NOT NULL,
    posicion_z REAL NOT NULL,

    activa_por_defecto INTEGER NOT NULL DEFAULT 0,
    seleccionable_aleatoria INTEGER NOT NULL DEFAULT 1,

    FOREIGN KEY (id_nivel)
        REFERENCES nivel(id_nivel)
        ON UPDATE CASCADE
        ON DELETE CASCADE,

    FOREIGN KEY (id_tarea_config)
        REFERENCES tarea_config(id_tarea_config)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,

    UNIQUE (id_nivel, codigo)
);

-- =========================================================
-- 3. Partidas y runtime
-- =========================================================

CREATE TABLE partida (
    id_partida INTEGER PRIMARY KEY AUTOINCREMENT,

    id_jugador INTEGER NOT NULL,
    id_nivel INTEGER NOT NULL,

    estado TEXT NOT NULL
        CHECK (estado IN (
            'PREPARACION',
            'EN_CURSO',
            'ESCAPE',
            'VICTORIA',
            'DERROTA',
            'ABANDONADA'
        )) DEFAULT 'PREPARACION',

    tareas_completadas INTEGER NOT NULL DEFAULT 0,
    fase_escape_activa INTEGER NOT NULL DEFAULT 0,

    corazones_restantes INTEGER,
    stamina_actual REAL,

    inicio_en TEXT NOT NULL DEFAULT (STRFTIME('%Y-%m-%d %H:%M:%S', 'now')),
    fin_en TEXT,
    duracion_segundos INTEGER,

    FOREIGN KEY (id_jugador)
        REFERENCES jugador(id_jugador)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,

    FOREIGN KEY (id_nivel)
        REFERENCES nivel(id_nivel)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,

    CHECK (tareas_completadas >= 0),
    CHECK (fin_en IS NULL OR fin_en >= inicio_en)
);

CREATE TABLE partida_tarea (
    id_partida_tarea INTEGER PRIMARY KEY AUTOINCREMENT,

    id_partida INTEGER NOT NULL,
    id_nivel_tarea INTEGER NOT NULL,

    estado TEXT NOT NULL
        CHECK (estado IN (
            'OCULTA',
            'ACTIVA',
            'EN_PROGRESO',
            'COMPLETADA',
            'CANCELADA'
        )) DEFAULT 'OCULTA',

    progreso REAL NOT NULL DEFAULT 0.00,
    tiempo_restante REAL,

    qte_exitosos INTEGER NOT NULL DEFAULT 0,
    qte_fallidos INTEGER NOT NULL DEFAULT 0,

    inicio_en TEXT,
    completada_en TEXT,

    FOREIGN KEY (id_partida)
        REFERENCES partida(id_partida)
        ON UPDATE CASCADE
        ON DELETE CASCADE,

    FOREIGN KEY (id_nivel_tarea)
        REFERENCES nivel_tarea(id_nivel_tarea)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,

    UNIQUE (id_partida, id_nivel_tarea),

    CHECK (progreso BETWEEN 0.00 AND 100.00)
);

CREATE TABLE partida_enemigo (
    id_partida_enemigo INTEGER PRIMARY KEY AUTOINCREMENT,

    id_partida INTEGER NOT NULL,
    id_enemigo_config INTEGER NOT NULL,
    id_punto_spawn INTEGER,

    estado_ia TEXT NOT NULL
        CHECK (estado_ia IN (
            'PATRULLA',
            'PERSECUCION',
            'ATAQUE',
            'COOLDOWN',
            'RANGED_WINDUP',
            'GRAB_WINDUP',
            'INACTIVO'
        )) DEFAULT 'PATRULLA',

    posicion_x REAL,
    posicion_y REAL,
    posicion_z REAL,

    velocidad_actual REAL,
    cooldown_ataque_restante REAL,
    cooldown_disparo_restante REAL,
    cooldown_agarre_restante REAL,

    activo INTEGER NOT NULL DEFAULT 1,

    FOREIGN KEY (id_partida)
        REFERENCES partida(id_partida)
        ON UPDATE CASCADE
        ON DELETE CASCADE,

    FOREIGN KEY (id_enemigo_config)
        REFERENCES enemigo_config(id_enemigo_config)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,

    FOREIGN KEY (id_punto_spawn)
        REFERENCES punto_spawn(id_punto_spawn)
        ON UPDATE CASCADE
        ON DELETE SET NULL
);

CREATE TABLE evento_qte (
    id_evento_qte INTEGER PRIMARY KEY AUTOINCREMENT,

    id_partida_tarea INTEGER NOT NULL,

    resultado TEXT NOT NULL
        CHECK (resultado IN ('EXITOSO', 'FALLIDO', 'EXPIRADO')),

    posicion_barra REAL,
    zona_inicio REAL,
    zona_fin REAL,

    bonificacion_aplicada REAL NOT NULL DEFAULT 0.00,

    creado_en TEXT NOT NULL DEFAULT (STRFTIME('%Y-%m-%d %H:%M:%S', 'now')),

    FOREIGN KEY (id_partida_tarea)
        REFERENCES partida_tarea(id_partida_tarea)
        ON UPDATE CASCADE
        ON DELETE CASCADE,

    CHECK (bonificacion_aplicada >= 0)
);

CREATE TABLE evento_danio (
    id_evento_danio INTEGER PRIMARY KEY AUTOINCREMENT,

    id_partida INTEGER NOT NULL,
    id_partida_enemigo INTEGER,
    id_proyectil_config INTEGER,

    origen TEXT NOT NULL
        CHECK (origen IN (
            'CONTACTO_ENEMIGO',
            'PROYECTIL',
            'AGARRE'
        )),

    cantidad INTEGER NOT NULL DEFAULT 1,
    corazones_restantes INTEGER,

    posicion_x REAL,
    posicion_y REAL,
    posicion_z REAL,

    creado_en TEXT NOT NULL DEFAULT (STRFTIME('%Y-%m-%d %H:%M:%S', 'now')),

    FOREIGN KEY (id_partida)
        REFERENCES partida(id_partida)
        ON UPDATE CASCADE
        ON DELETE CASCADE,

    FOREIGN KEY (id_partida_enemigo)
        REFERENCES partida_enemigo(id_partida_enemigo)
        ON UPDATE CASCADE
        ON DELETE SET NULL,

    FOREIGN KEY (id_proyectil_config)
        REFERENCES proyectil_config(id_proyectil_config)
        ON UPDATE CASCADE
        ON DELETE SET NULL,

    CHECK (cantidad > 0)
);

-- =========================================================
-- DATOS INICIALES (DML)
-- =========================================================

INSERT INTO jugador (nombre_usuario)
VALUES ('Jugador_Prueba');

INSERT INTO nivel (
    nombre,
    escena_unity,
    total_tareas_requeridas,
    tareas_activas_por_partida,
    multiplicador_velocidad_escape,
    descripcion
)
VALUES (
    'SampleScene',
    'Assets/Scenes/SampleScene.unity',
    3,
    3,
    1.50,
    'Escena principal'
);

INSERT INTO configuracion_jugador (
    id_nivel,
    velocidad_caminar,
    velocidad_correr,
    gravedad,
    velocidad_giro,
    stamina_max,
    tasa_regeneracion_stamina,
    tasa_consumo_stamina,
    corazones_iniciales
)
VALUES (
    (SELECT id_nivel
     FROM nivel
     WHERE escena_unity = 'Assets/Scenes/SampleScene.unity'),

    5.00,
    9.00,
    -9.81,
    10.00,
    100.00,
    20.00,
    25.00,
    3
);

INSERT INTO puerta_salida (
    id_nivel,
    nombre,
    offset_x,
    offset_y,
    offset_z,
    velocidad_apertura
)
VALUES (
    (SELECT id_nivel
     FROM nivel
     WHERE escena_unity = 'Assets/Scenes/SampleScene.unity'),

    'Door',
    3.000,
    0.000,
    0.000,
    2.00
);

INSERT INTO tipo_enemigo (codigo, nombre, descripcion)
VALUES
('BASE', 'Enemigo base', 'Patrulla y ataque cuerpo a cuerpo'),
('RANGED', 'Enemigo a distancia', 'Dispara proyectiles'),
('GRAB', 'Enemigo de agarre', 'Atrapa al jugador');

PRAGMA foreign_keys = ON;
