create database catalogo_autos;
USE catalogo_autos;

CREATE TABLE marca (
  id INT PRIMARY KEY,
  nombre VARCHAR(255)
);

CREATE TABLE submarca (
  id INT PRIMARY KEY,
  marca_id INT,
  nombre VARCHAR(255),
  FOREIGN KEY (marca_id) REFERENCES marca(id)
);

CREATE TABLE modelo (
  id INT PRIMARY KEY,
  submarca_id INT,
  ano INT,
  FOREIGN KEY (submarca_id) REFERENCES submarca(id)
);

CREATE TABLE descripcion (
  id INT PRIMARY KEY,
  modelo_id INT,
  detalles VARCHAR(255),
  FOREIGN KEY (modelo_id) REFERENCES modelo(id)
);

BULK INSERT marca
FROM 'C:\automoviles.txt'
WITH (
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '\n'
);

select * from marca


SELECT DISTINCT nombre FROM marca

SELECT * FROM marca

SELECT DISTINCT marca FROM catalogoE

SELECT DISTINCT Submarca FROM catalogoE WHERE Marca = 'acura'

SELECT DISTINCT Modelo FROM catalogoE WHERE Submarca = 'ILX'

SELECT descripcion, DescripcionId FROM catalogoE WHERE Submarca = 'MAZDA 3' and Modelo = '2015'

SELECT DISTINCT marca FROM catalogoE