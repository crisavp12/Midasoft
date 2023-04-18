USE [PruebaTecnicaMidasoft]
GO
/****** Object:  Table [dbo].[familiar]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[familiar](
	[FamiliarId] [int] IDENTITY(1,1) NOT NULL,
	[Usuario] [nvarchar](50) NOT NULL,
	[Cedula] [nvarchar](max) NOT NULL,
	[Nombres] [nvarchar](max) NOT NULL,
	[Apellidos] [nvarchar](max) NOT NULL,
	[Genero] [nvarchar](max) NULL,
	[Parentesco] [nvarchar](max) NULL,
	[Edad] [int] NOT NULL,
	[MenorEdad] [bit] NULL,
	[FechaNacimiento] [datetime] NULL,
 CONSTRAINT [PK__familiar__267867938584D5E7] PRIMARY KEY CLUSTERED 
(
	[FamiliarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Log]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Peticion] [nvarchar](max) NOT NULL,
	[Respuesta] [nvarchar](max) NOT NULL,
	[Fecha] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[usuario]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[usuario](
	[Usuario] [nvarchar](50) NOT NULL,
	[Contrasena] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[SPCreateFamiliar]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPCreateFamiliar]
    @Usuario NVARCHAR(50),
    @Cedula NVARCHAR(50),
    @Nombres NVARCHAR(100),
    @Apellidos NVARCHAR(100),
    @Genero NVARCHAR(50) = null,
    @Parentesco NVARCHAR(50) = null,
    @Edad INT,
    @MenorEdad bit = null,
    @FechaNacimiento DATE = null
AS
BEGIN
    -- cuerpo del procedimiento almacenado
    INSERT INTO dbo.familiar (Usuario, Cedula, Nombres, Apellidos, Genero, Parentesco, Edad, MenorEdad, FechaNacimiento)
    VALUES (@Usuario, @Cedula, @Nombres, @Apellidos, @Genero, @Parentesco, @Edad, @MenorEdad, @FechaNacimiento)
END
GO
/****** Object:  StoredProcedure [dbo].[SPCreateUser]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPCreateUser]
    @Usuario NVARCHAR(50),
    @Contrasena NVARCHAR(MAX)
AS
BEGIN
    -- Insertar el nuevo familiar en la tabla correspondiente
    INSERT INTO usuario (Usuario, Contrasena)
    VALUES (@Usuario, @Contrasena)

    -- Retornar el número de filas afectadas
    RETURN @@ROWCOUNT
END
GO
/****** Object:  StoredProcedure [dbo].[SPDeleteFamiliar]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPDeleteFamiliar]
    @FamiliarId INT
AS
BEGIN
    DELETE FROM dbo.familiar WHERE FamiliarId = @FamiliarId
END
GO
/****** Object:  StoredProcedure [dbo].[SPDeleteUser]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPDeleteUser]
    @Usuario NVARCHAR(50)
AS
BEGIN
    -- Verificar si el usuario existe antes de eliminarlo
    IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE Usuario = @Usuario)
    BEGIN
        BEGIN TRY
            -- Iniciar una transacción
            BEGIN TRANSACTION

            -- Eliminar el usuario
            DELETE FROM dbo.Usuario WHERE Usuario = @Usuario

            -- Confirmar la transacción
            COMMIT

            -- Retornar el número de filas afectadas (1 si el usuario fue eliminado exitosamente)
            SELECT @@ROWCOUNT AS 'RowsAffected'
        END TRY
        BEGIN CATCH
            -- Si ocurre un error, deshacer la transacción
            ROLLBACK

            -- Lanzar una excepción con el mensaje de error
            DECLARE @ErrorMessage NVARCHAR(500)
            SET @ErrorMessage = ERROR_MESSAGE()
            RAISERROR(@ErrorMessage, 16, 1)
        END CATCH
    END
    ELSE
    BEGIN
        -- Si el usuario no existe, retornar un valor indicando que no se eliminó ninguna fila
        SELECT 0 AS 'RowsAffected'
    END
END
GO
/****** Object:  StoredProcedure [dbo].[SPEditFamiliar]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPEditFamiliar]
    @FamiliarId INT,
    @Usuario NVARCHAR(50),
    @Cedula NVARCHAR(50),
    @Nombres NVARCHAR(50),
    @Apellidos NVARCHAR(50),
    @Genero NVARCHAR(50) = null,
    @Parentesco NVARCHAR(50) = null,
    @Edad INT,
    @MenorEdad bit = null,
    @FechaNacimiento DATE = null
AS
BEGIN
    UPDATE dbo.familiar
    SET Usuario = @Usuario, 
        Cedula = @Cedula, 
        Nombres = @Nombres, 
        Apellidos = @Apellidos, 
        Genero = @Genero, 
        Parentesco = @Parentesco, 
        Edad = @Edad, 
        MenorEdad = @MenorEdad, 
        FechaNacimiento = @FechaNacimiento
    WHERE FamiliarId = @FamiliarId;
    
    -- Retornar el número de filas afectadas por la actualización
    RETURN @@ROWCOUNT;
END;
GO
/****** Object:  StoredProcedure [dbo].[SPEditUserPassword]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPEditUserPassword]
    @Usuario NVARCHAR(50),
    @Contrasena NVARCHAR(MAX)
AS
BEGIN
    -- Actualizar la contraseña del usuario en la base de datos
    UPDATE Usuario
    SET Contrasena = @Contrasena
    WHERE Usuario = @Usuario;

    -- Devolver el número de filas afectadas
    SELECT @@ROWCOUNT;
END
GO
/****** Object:  StoredProcedure [dbo].[SPGetFamiliarByDocument]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetFamiliarByDocument]
    @cedula NVARCHAR(50)
AS
BEGIN
    -- Realizar la consulta para verificar si existe un familiar con la cédula especificada
    SELECT 1
    FROM Familiar
    WHERE Cedula = @cedula
END
GO
/****** Object:  StoredProcedure [dbo].[SPGetFamiliarById]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[SPGetFamiliarById]
    @FamiliarId INT
AS
BEGIN
    -- cuerpo del procedimiento almacenado
    SELECT FamiliarId, Usuario, Cedula, Nombres, Apellidos, Genero, Parentesco, Edad, MenorEdad, FechaNacimiento
    FROM dbo.familiar
    WHERE FamiliarId = @FamiliarId
END
GO
/****** Object:  StoredProcedure [dbo].[SPGetFamiliarByUser]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetFamiliarByUser]
    @usuario varchar(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si existe un familiar con el ID proporcionado
    IF EXISTS (SELECT 1 FROM Familiar WHERE Usuario = @usuario)
        SELECT 1 AS 'Existe' -- Si existe, devolver 1
    ELSE
        SELECT 0 AS 'Existe' -- Si no existe, devolver 0
END

GO
/****** Object:  StoredProcedure [dbo].[SPGetFamiliares]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetFamiliares]
AS
BEGIN
    SELECT FamiliarId, Usuario, Cedula, Nombres, Apellidos, Genero, Parentesco, Edad, MenorEdad, FechaNacimiento
    FROM dbo.familiar
END
GO
/****** Object:  StoredProcedure [dbo].[SPGetUsuarioByLogin]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPGetUsuarioByLogin]
    @Usuario NVARCHAR(50)
AS
BEGIN
    SELECT [Usuario], [Contrasena]
    FROM [dbo].[usuario]
    WHERE [Usuario] = @Usuario
END
GO
/****** Object:  StoredProcedure [dbo].[SPGetUsuarios]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetUsuarios]
AS
BEGIN
    SELECT Usuario
    FROM Usuario -- Reemplaza "Usuarios" con el nombre de tu tabla de usuarios
END
GO
/****** Object:  StoredProcedure [dbo].[SPSaveLog]    Script Date: 17/04/2023 20:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPSaveLog]
    @Peticion NVARCHAR(MAX),
    @Respuesta NVARCHAR(MAX)
AS
BEGIN
    -- Insertar el registro en la tabla Log
    INSERT INTO Log (Peticion, Respuesta, Fecha)
    VALUES (@Peticion, @Respuesta, GETDATE())

    -- Obtener el ID del registro insertado
    DECLARE @Id INT
    SET @Id = SCOPE_IDENTITY()

    -- Retornar el ID del registro insertado
    SELECT @Id AS 'Id'
END
GO
