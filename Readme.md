The Moogle! search engine
===========================

![](moogle.png)

## Cuál es el propósito de este software_
Es un intérprete para el lenguaje de programación HULK.
  
## Requerimientos
Dotnet 6.0

## Portabilidad

Funciona en GNU/Linux.

## Guía rápida
1. 

    cd InteractiveConsole

2.

    dotnet run

3. 

    print("hello world")
    
## Testing
```
cd Tests
dotnet test     
```


## Cómo funciona?
1. El Lexer lee el texto enviado y lo convierte en tokens
2. El parser recibe los tokens y establece las relaciones entre las diferentes estructuras (árboles de sintaxis abstractos)
3. El intérprete recibe el árbol de sintaxis y lo evalúa pasándole el contexto global.
