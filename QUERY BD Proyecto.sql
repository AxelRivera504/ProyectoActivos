CREATE DATABASE ACTIVOS
GO
USE ACTIVOS
GO
CREATE SCHEMA Acce
GO
CREATE SCHEMA Gral
GO
CREATE SCHEMA Acti
GO

--TABLES

CREATE TABLE Acce.tbUsuarios(
	usua_Id					INT IDENTITY(1,1),
	usua_UsuarioNombre		NVARCHAR(200) NOT NULL,
	usua_UsuarioApellido	NVARCHAR(200) NOT NULL,
	usua_Usuario			NVARCHAR(200) NOT NULL,
	usua_Contra				NVARCHAR(MAX) NOT NULL,

	usua_UsuarioCreacion	INT NOT NULL,
	usua_FechaCreacion		DATETIME NOT  NULL,
	usua_Modificacion		INT,
	usua_FechaModificacion	DATETIME,
	usua_Estado				BIT DEFAULT (0)
	CONSTRAINT PK_Acce_tbUsuarios_usua_Id PRIMARY KEY(usua_Id) 
);

GO

CREATE TABLE Gral.tbClientes(
	clie_Id				INT IDENTITY(1,1), 
	clie_CodigoCompania	NVARCHAR(200) NOT NULL,
	clie_NombreCliente	NVARCHAR(200) NOT NULL,
	clie_RTNCliente		NVARCHAR(16) NOT NULL,
	clie_Lugar			NVARCHAR(500) NOT NULL,
	clie_Responsale		NVARCHAR(200) NOT NULL,
	clie_Telefono		NVARCHAR(15) NOT NULL,

	usua_UsuarioCreacion	INT NOT NULL,
	clie_FechaCreacion		DATETIME NOT  NULL,
	usua_Modificacion		INT,
	clie_FechaModificacion	DATETIME,
	clie_Estado				BIT DEFAULT (0)
	CONSTRAINT PK_Gral_tbCliente_clie_Id PRIMARY KEY(clie_Id)
);
GO

CREATE TABLE Acti.tbVidaUtil(
	viut_Id					INT IDENTITY(1,1),
	viut_Objeto				NVARCHAR(6)NOT NULL,
	viut_Descripcion		NVARCHAR(150)NOT NULL,
	viut_VidaUtil			INT NOT NULL,

	usua_UsuarioCreacion	INT NOT NULL,
	viut_FechaCreacion		DATETIME NOT  NULL,
	usua_Modificacion		INT,
	viut_FechaModificacion	DATETIME,
	viut_Estado				BIT

	CONSTRAINT PK_Acti_tbVidaUtil_viut_Id PRIMARY KEY(viut_Id)
);
GO

CREATE TABLE Acti.tbActivos(
	acvo_Id					INT IDENTITY(1,1),
	acvo_Nombre				NVARCHAR(200) NOT NULL,
	acvo_NumeroSerie		NVARCHAR(20) NULL,
	viut_Id					INT NOT NULL,
	clie_Id					INT NOT NULL,
	acvo_Ubicacion			NVARCHAR(150) NOT NULL,
	acvo_FechaAdquisicion	DATE NOT NULL,
	acvo_InicioDepreciacion	DATE NOT NULL,
	acvo_CostoOriginal		DECIMAL(18,2) NOT NULL,
	acvo_ValorResidual		DECIMAL(18,2) NOT NULL,
	acvo_CostoDespreciable	DECIMAL(18,2) NOT NULL,


	usua_UsuarioCreacion	INT NOT NULL,
	acvo_FechaCreacion		DATETIME NOT  NULL,
	usua_Modificacion		INT,
	acvo_FechaModificacion	DATETIME,
	acvo_Estado				BIT DEFAULT (0),
	acvo_FechaBajada DATE NULL

	CONSTRAINT PK_Acti_tbActivos_acvo_Id PRIMARY KEY(acvo_Id),
	CONSTRAINT FK_Acti_tbActivos_Acti_tbVidaUtil_viut_Id FOREIGN KEY(viut_Id) REFERENCES Acti.tbVidaUtil(viut_Id),
	CONSTRAINT FK_Acti_tbActivos_Gral_tbCliente_clac_Id FOREIGN KEY(clie_Id) REFERENCES Gral.tbClientes(clie_Id)
);
GO

--ALTERS

--USUARIOS
ALTER TABLE Acce.tbUsuarios
ADD CONSTRAINT FK_Acce_tbUsuarios_usua_UsuarioCreacion FOREIGN KEY (usua_UsuarioCreacion) REFERENCES Acce.tbUsuarios(usua_Id)
GO
ALTER TABLE Acce.tbUsuarios
ADD CONSTRAINT FK_Acce_tbUsuarios_usua_Modificacion FOREIGN KEY (usua_Modificacion) REFERENCES Acce.tbUsuarios(usua_Id)

GO

--CLIENTES
ALTER TABLE Gral.tbCliente
ADD CONSTRAINT FK_Gral_tbCliente_usua_UsuarioCreacion FOREIGN KEY (usua_UsuarioCreacion) REFERENCES Acce.tbUsuarios(usua_Id)
GO
ALTER TABLE Gral.tbCliente
ADD CONSTRAINT FK_Gral_tbCliente_usua_Modificacion FOREIGN KEY (usua_Modificacion) REFERENCES Acce.tbUsuarios(usua_Id)

GO

--CLASE ACTIVOS
ALTER TABLE Acti.tbVidaUtil
ADD CONSTRAINT FK_Acti_tbVidaUtil_usua_UsuarioCreacion FOREIGN KEY (usua_UsuarioCreacion) REFERENCES Acce.tbUsuarios(usua_Id)
GO
ALTER TABLE Acti.tbClaseActivo
ADD CONSTRAINT FK_Acti_tbVidaUtil_usua_Modificacion FOREIGN KEY (usua_Modificacion) REFERENCES Acce.tbUsuarios(usua_Id)

GO

--ACTIVOS
ALTER TABLE Acti.tbActivos
ADD CONSTRAINT FK_Acti_tbActivos_usua_UsuarioCreacion FOREIGN KEY (usua_UsuarioCreacion) REFERENCES Acce.tbUsuarios(usua_Id)
GO
ALTER TABLE Acti.tbActivos
ADD CONSTRAINT FK_Acti_tbActivos_usua_Modificacion FOREIGN KEY (usua_Modificacion) REFERENCES Acce.tbUsuarios(usua_Id)

GO


--UDP'S
GO

--Procedimientos almacenados Usuarios y  login

CREATE OR ALTER PROCEDURE Acce.UDP_tbusuarios_CambiarPassword
	@usua_Usuario	NVARCHAR(MAX),
	@usua_Contra		NVARCHAR(MAX)
AS
	BEGIN		
		BEGIN TRY
			DECLARE @Pass AS NVARCHAR(MAX);
			SET @Pass = CONVERT(NVARCHAR(MAX), HASHBYTES('sha2_512', @usua_Contra), 2);

					
			UPDATE  Acce.tbUsuarios 
				SET  usua_Contra  = @Pass
				WHERE  usua_Usuario = @usua_Usuario

			SELECT 1
		END TRY
		BEGIN CATCH
			SELECT 0
		END CATCH
	END
GO

CREATE OR ALTER PROCEDURE Acce.UDP_tbUsuarios_VerificarUsuarios
		@usua_Usuario	NVARCHAR(200)
AS
BEGIN
	BEGIN TRY
		IF EXISTS (SELECT usua_Id FROM Acce.tbUsuarios WHERE usua_Usuario = @usua_Usuario)
			BEGIN
				SELECT 1
			END
		ELSE
			BEGIN
				SELECT 0
			END
	END TRY
	BEGIN CATCH
		SELECT -2
	END CATCH
	
END

GO

CREATE OR ALTER PROCEDURE Acce.UDP_IniciarSesion
	@usua_Usuario				NVARCHAR(200),
	@usua_Contra				NVARCHAR(MAX)
AS
BEGIN
	BEGIN TRY
		DECLARE @Pass NVARCHAR(MAX) = CONVERT(NVARCHAR(MAX), HASHBYTES('sha2_512', @usua_Contra), 2);
		IF EXISTS (SELECT * 
				   FROM Acce.tbUsuarios 
				   WHERE usua_Usuario = @usua_Usuario 
				   AND usua_Contra = @Pass
				   AND usua_Estado = 1)
			BEGIN
				SELECT		usua.usua_Id,
							usua.usua_UsuarioNombre,
							usua.usua_UsuarioApellido,
							usua.usua_Usuario
					FROM	Acce.tbUsuarios usua
					WHERE   usua_Usuario = @usua_Usuario
			END
		ELSE
			BEGIN
				SELECT 0
			END
	END TRY
	BEGIN CATCH
		SELECT 'Error Message: ' + ERROR_MESSAGE()
	END CATCH
END
GO


CREATE OR ALTER PROCEDURE Acce.UDP_tbUsuarios_ListarUsuarios
AS
BEGIN
	SELECT	usua_Id,
			usua_UsuarioNombre,
			usua_UsuarioApellido,
			usua_Usuario,
			usua_UsuarioCreacion,
			usua_FechaCreacion,
			usua_Modificacion,
			usua_FechaModificacion,
			usua_Estado
	FROM	Acce.tbUsuarios

END

GO

CREATE OR ALTER PROCEDURE Acce.UDP_tbusuarios_InsertarUsuario
@usua_UsuarioNombre			NVARCHAR(100),
@usua_UsuarioApellido		NVARCHAR(100),
@usua_Usuario				NVARCHAR(200),
@usua_Contra				NVARCHAR(MAX),
@usua_UsuarioCreacion		INT
AS
BEGIN
	BEGIN TRY
		IF EXISTS (SELECT * FROM Acce.tbUsuarios WHERE usua_Usuario = @usua_Usuario AND usua_Estado = 1)
			BEGIN
				SELECT 0
			END
		ELSE 
			BEGIN
				DECLARE @Pass AS NVARCHAR(MAX);
				SET @Pass = CONVERT(NVARCHAR(MAX), HASHBYTES('sha2_512', @usua_Contra), 2);

				INSERT INTO  Acce.tbUsuarios(usua_UsuarioNombre, usua_UsuarioApellido, usua_Usuario, usua_Contra, usua_UsuarioCreacion, usua_FechaCreacion, usua_Modificacion, usua_FechaModificacion, usua_Estado)
				VALUES (@usua_UsuarioNombre,@usua_UsuarioApellido,@usua_Usuario,@Pass,@usua_UsuarioCreacion, GETDATE(), NULL, NULL,1)

				SELECT	1
			END
	END TRY
	BEGIN CATCH
		SELECT -2
	END CATCH
END

GO

CREATE OR ALTER PROCEDURE Acce.UDP_tbusuarios_EditarUsuario 
@usua_Id					INT,
@usua_UsuarioNombre			NVARCHAR(100),
@usua_UsuarioApellido		NVARCHAR(100),
@usua_UsuarioModificacion	INT
AS
BEGIN
	BEGIN TRY
			UPDATE  [Acce].[tbUsuarios]
			SET		usua_UsuarioNombre = @usua_UsuarioNombre,
					usua_UsuarioApellido = @usua_UsuarioApellido,
					usua_Modificacion = @usua_UsuarioModificacion,
					usua_FechaModificacion = GETDATE()
			WHERE usua_Id = @usua_Id
				SELECT	1
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH
END

GO

CREATE OR ALTER PROCEDURE Acce.UDP_tbUsuarios_DeshabilitarUsuarios
	@usua_Id		INT
AS
BEGIN
	BEGIN TRY
		UPDATE [Acce].[tbUsuarios]
			   SET [usua_Estado] = 0
		WHERE	usua_Id = @usua_Id

		SELECT 1
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE Acce.UDP_tbUsuarios_HabilitarUsuarios
	@usua_Id		INT
AS
BEGIN
	BEGIN TRY
		UPDATE [Acce].[tbUsuarios]
			   SET [usua_Estado] = 1
		WHERE	usua_Id = @usua_Id

		SELECT 1
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH
END
GO

--Clientes
CREATE OR ALTER PROCEDURE Gral.UDP_tbClientes_ListarClientes
AS
BEGIN
	SELECT	clie_Id,
			clie_CodigoCompania,
			clie_NombreCliente,
			clie_Responsale,
			clie_RTNCliente,
			clie_Lugar,
			clie_Telefono,
			clie_FechaCreacion,
			clie_FechaModificacion
	FROM	Gral.tbClientes
	WHERE	clie_Estado = 1
END

GO

CREATE OR ALTER PROCEDURE Gral.UDP_tbClientes_ValidarRTN
	@clie_RTNCliente			NVARCHAR(16)
AS
BEGIN
	BEGIN TRY
		IF EXISTS(SELECT clie_Id FROM Gral.tbClientes WHERE clie_RTNCliente = @clie_RTNCliente)
			BEGIN
				SELECT 1
			END
		ELSE
			BEGIN
				SELECT 2
			END
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH
END


GO

CREATE OR ALTER PROCEDURE Gral.UDP_tbClientes_InsertarClientes 
	@clie_CodigoCompania		NVARCHAR(200),
	@clie_NombreCliente			NVARCHAR(200),
	@clie_RTNCliente			NVARCHAR(16),
	@clie_Lugar					NVARCHAR(500),
	@clie_Responsale			NVARCHAR(200),
	@clie_Telefono				NVARCHAR(15),
	@usua_UsuarioCreacion		INT
AS
BEGIN
	BEGIN TRY
		INSERT INTO GRAL.tbClientes(clie_CodigoCompania, clie_NombreCliente, clie_RTNCliente, clie_Lugar, clie_Responsale, clie_Telefono, usua_UsuarioCreacion, clie_FechaCreacion, usua_Modificacion, clie_FechaModificacion, clie_Estado)
		VALUES(@clie_CodigoCompania,@clie_NombreCliente,@clie_RTNCliente,@clie_Lugar,@clie_Responsale,@clie_Telefono,@usua_UsuarioCreacion,GETDATE(),NULL,NULL,1);

		SELECT 1
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH
END

GO

CREATE OR ALTER PROCEDURE Gral.UDP_tbClientes_EditarClientes 
	@clie_Id					INT,
	@clie_CodigoCompania		NVARCHAR(200),
	@clie_NombreCliente			NVARCHAR(200),
	@clie_RTNCliente			NVARCHAR(16),
	@clie_Lugar					NVARCHAR(500),
	@clie_Responsale			NVARCHAR(200),
	@clie_Telefono				NVARCHAR(15),
	@usua_Modificacion			INT
AS
BEGIN
	BEGIN TRY	
		UPDATE [GRAL].[tbClientes]
		   SET [clie_CodigoCompania] =@clie_CodigoCompania
			  ,[clie_NombreCliente] =@clie_NombreCliente
			  ,[clie_RTNCliente] = @clie_RTNCliente
			  ,[clie_Lugar] = @clie_Lugar
			  ,[clie_Responsale] =@clie_Responsale
			  ,[clie_Telefono] =@clie_Telefono
			  ,[usua_Modificacion] = @usua_Modificacion
			  ,[clie_FechaModificacion] = GETDATE()
		 WHERE clie_Id = @clie_Id

		SELECT 1
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH
END

GO

--Clase Vida Util
CREATE OR ALTER PROCEDURE Acti.UDP_tbVidaUtil_ListarVidaUtil
AS
BEGIN
	BEGIN TRY
		SELECT	 viut_Id,
				 viut_Objeto,
				 viut_Descripcion,
				 viut_VidaUtil,
				 viut_Estado
		FROM     Acti.tbVidaUtil
		WHERE	 viut_Estado = 1
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH
END

GO

CREATE OR ALTER PROCEDURE Acti.UDP_tbVidaUtil_Insertar
	@viut_Objeto		NVARCHAR(6),
	@viut_Descripcion	NVARCHAR(200),
	@viut_VidaUtil		INT,
	@usuarioCreacion	INT
AS
BEGIN
	BEGIN TRY
		IF EXISTS (SELECT viut_Id FROM Acti.tbVidaUtil WHERE viut_Objeto = @viut_Objeto AND viut_VidaUtil = @viut_VidaUtil)
			BEGIN
				SELECT 2
			END
		ELSE
			BEGIN
				INSERT INTO Acti.tbVidaUtil(viut_Objeto,viut_Descripcion,viut_VidaUtil,usua_UsuarioCreacion,viut_FechaCreacion,usua_Modificacion,viut_FechaModificacion,viut_Estado)
				VALUES(@viut_Objeto,@viut_Descripcion,@viut_VidaUtil,@usuarioCreacion,GETDATE(),NULL,NULL,1);
				SELECT 1
			END		
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH
END

GO

CREATE OR ALTER PROCEDURE Acti.UDP_tbVidaUtil_Editar
	@viut_Id				INT,
	@viut_Objeto			NVARCHAR(6),
	@viut_Descripcion		NVARCHAR(200),
	@viut_VidaUtil			INT,
	@usuarioModificacion	INT
AS
BEGIN
	BEGIN TRY
		UPDATE  Acti.tbVidaUtil
		SET		viut_Objeto = @viut_Objeto,
				viut_Descripcion = @viut_Descripcion,
				viut_VidaUtil = @viut_VidaUtil,
				usua_Modificacion = @usuarioModificacion,
				viut_FechaModificacion = GETDATE()
		WHERE	viut_Id = @viut_Id

		SELECT 1
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH
END

GO

--Activos
CREATE OR ALTER PROCEDURE Acti.UDP_tbActivos_ListarActivos
AS
BEGIN
	BEGIN TRY
		SELECT	acvo_Id,
				acvo_Nombre,
				acvo_NumeroSerie,
				actv.viut_Id,
				vitu.viut_VidaUtil,
				actv.clie_Id,
				clie.clie_NombreCliente,
				clie.clie_CodigoCompania,
				acvo_Ubicacion,
				acvo_FechaAdquisicion,
				acvo_InicioDepreciacion,
				acvo_CostoOriginal,
				acvo_ValorResidual,
				acvo_CostoDespreciable,
				acvo_Estado,
				acvo_FechaCreacion,
				acvo_FechaModificacion
		FROM	Acti.tbActivos actv INNER JOIN Acti.tbVidaUtil vitu
		ON		actv.viut_Id = vitu.viut_Id INNER JOIN Gral.tbClientes clie
		ON		actv.clie_Id = clie.clie_Id

	END TRY
	BEGIN CATCH	
		SELECT 0
	END CATCH
END

GO

CREATE OR ALTER PROCEDURE Acti.UDP_tbActivos_InsertarActivos
	@acvo_Nombre			NVARCHAR(200), 
	@acvo_NumeroSerie		NVARCHAR(200), 
	@viut_Id				INT, 
	@clie_Id				INT, 
	@acvo_Ubicacion			NVARCHAR(200), 
	@acvo_FechaAdquisicion	DATE,
	@acvo_CostoOriginal		DECIMAL(18,4),
	@usua_UsuarioCreacion	INT 
AS
BEGIN
	BEGIN TRY
			DECLARE @DiaDelMesIngresado INT = DAY(@acvo_FechaAdquisicion);
			DECLARE @UltimoDiaMes INT = DAY(EOMONTH( @acvo_FechaAdquisicion ));
			DECLARE @FechaMasisa DATE;
			IF(@DiaDelMesIngresado <= 14)
				BEGIN
					SET @FechaMasisa = DATEADD(DAY, 1 - @DiaDelMesIngresado, @acvo_FechaAdquisicion)
				END
			ELSE
				BEGIN
					SET @FechaMasisa = DATEADD(MONTH, 1, @acvo_FechaAdquisicion); 
					SET @FechaMasisa = DATEADD(DAY, -1 * DAY(@FechaMasisa) + 1, @FechaMasisa);
				END
		
			DECLARE @acvo_ValorResidual DECIMAL(18,4) = @acvo_CostoOriginal * 0.01;
			DECLARE @acvo_CostoDespreciable DECIMAL(18,4) = @acvo_CostoOriginal - @acvo_ValorResidual;

			INSERT INTO [Acti].[tbActivos](acvo_Nombre, acvo_NumeroSerie, viut_Id, clie_Id, acvo_Ubicacion, acvo_FechaAdquisicion, acvo_InicioDepreciacion, acvo_CostoOriginal, acvo_ValorResidual, acvo_CostoDespreciable, usua_UsuarioCreacion, acvo_FechaCreacion, usua_Modificacion, acvo_FechaModificacion, acvo_Estado)
			VALUES(@acvo_Nombre,@acvo_NumeroSerie,@viut_Id,@clie_Id,@acvo_Ubicacion,@acvo_FechaAdquisicion,@FechaMasisa,@acvo_CostoOriginal,@acvo_ValorResidual , @acvo_CostoDespreciable,@usua_UsuarioCreacion,GETDATE(),NULL,NULL,1);
		SELECT 1
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH

 END

 
GO

CREATE OR ALTER PROCEDURE Acti.UDP_tbActivos_EditarActivos
	@acvo_Id				INT,
	@acvo_Nombre			NVARCHAR(200), 
	@acvo_NumeroSerie		NVARCHAR(200), 
	@viut_Id				INT, 
	@clie_Id				INT, 
	@acvo_Ubicacion			NVARCHAR(200), 
	@acvo_FechaAdquisicion	DATE,
	@acvo_CostoOriginal		DECIMAL(18,4),
	@usua_Modificacion		INT 
AS
BEGIN
	BEGIN TRY
			DECLARE @DiaDelMesIngresado INT = DAY(@acvo_FechaAdquisicion);
			DECLARE @UltimoDiaMes INT = DAY(EOMONTH( @acvo_FechaAdquisicion ));
			DECLARE @FechaMasisa DATE;
			IF(@DiaDelMesIngresado <= 14)
				BEGIN
					SET @FechaMasisa = DATEADD(DAY, 1 - @DiaDelMesIngresado, @acvo_FechaAdquisicion)
				END
			ELSE
				BEGIN
					SET @FechaMasisa = DATEADD(MONTH, 1, @acvo_FechaAdquisicion); 
					SET @FechaMasisa = DATEADD(DAY, -1 * DAY(@FechaMasisa) + 1, @FechaMasisa);
				END
		
			DECLARE @acvo_ValorResidual DECIMAL(18,4) = @acvo_CostoOriginal * 0.01;
			DECLARE @acvo_CostoDespreciable DECIMAL(18,4) = @acvo_CostoOriginal - @acvo_ValorResidual;

			UPDATE  [Acti].[tbActivos]
			SET     [acvo_Nombre] = @acvo_Nombre,
					[acvo_NumeroSerie] = @acvo_NumeroSerie,
					[viut_Id] = @viut_Id,
					[clie_Id] = @clie_Id,
					[acvo_Ubicacion] = @acvo_Ubicacion,
					[acvo_FechaAdquisicion] = @acvo_FechaAdquisicion,
					[acvo_InicioDepreciacion] = @FechaMasisa,
					[acvo_CostoOriginal] = @acvo_CostoOriginal,
					[acvo_ValorResidual] = @acvo_ValorResidual,
					[acvo_CostoDespreciable] = @acvo_CostoDespreciable,
					[usua_Modificacion] = @usua_Modificacion,
					[acvo_FechaModificacion] = GETDATE()
			WHERE	[acvo_Id] = @acvo_Id;

		SELECT 1
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH

 END


 GO


 CREATE OR ALTER PROCEDURE Acti.UDP_tbActivos_DarDeBajaActivo
	@acvo_Id	INT
 AS
 BEGIN
	BEGIN TRY
		UPDATE  [Acti].[tbActivos]
		SET		[acvo_Estado] = 0
		WHERE	[acvo_Id] = @acvo_Id

		SELECT 1
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH
 END

 GO

 CREATE OR ALTER PROCEDURE GenerarReportePorCliente
	@clie_Id		INT,
	@acvo_Estado	BIT
 AS
 BEGIN
	BEGIN TRY	
		 SELECT		acvo_Nombre, 
					vitu.viut_Descripcion,
					acvo_Ubicacion + ' - '+ ISNULL(acvo_NumeroSerie,'') as Ubicacion, 
					acvo_FechaAdquisicion, 
					acvo_InicioDepreciacion, 
					acvo_CostoOriginal,
					vitu.viut_VidaUtil,
					acvo_ValorResidual, 
					acvo_CostoDespreciable,

					CASE 
						WHEN MONTH(acvo_InicioDepreciacion) = 1
						THEN '0'
						ELSE CONCAT(CONVERT(NVARCHAR,(12 - MONTH(acvo_InicioDepreciacion) + 1 ),101),' Meses')
					END AS PeriodoMeses,

					 CASE 
						WHEN MONTH(acvo_InicioDepreciacion) = 1
						THEN '0.00%'
						ELSE CONCAT((CONVERT(NVARCHAR, (  CONVERT(DECIMAL(18,2), (ROUND(((CONVERT(DECIMAL(18,2),(1 / CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil))))*100) / 12) * (13 - MONTH(acvo_InicioDepreciacion)), 2)) )    ) ,101)),'%')    
					END AS TasaMensual,

					1 AS PeriodoA,

					 CONCAT(CONVERT(NVARCHAR,(  CONVERT(DECIMAL(18,2),(1 / CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil))))*100  ),101),'%')  AS TasaAnual,

					CASE 
						WHEN MONTH(acvo_InicioDepreciacion) = 1
						THEN '0'
						ELSE CONVERT(DECIMAL(18,2), ( ROUND(CONVERT(DECIMAL(18,2),(acvo_CostoOriginal - acvo_ValorResidual)) * ( ( ( ( (CONVERT( decimal(18,2),(1/ CONVERT( DECIMAL(18,2), (vitu.viut_VidaUtil) )   )  ))  ) / 12 ) ) * (13 - MONTH(acvo_InicioDepreciacion)) ) ,2) )  )    
					END AS DepPeriodoMeses,
			 
					CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    )    AS DepEnero,
					CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    )    AS DepFebrero,
					CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    )    AS DepMarzo,
					CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    )    AS DepAbril,
					CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    )    AS DepMayo,
					CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    )    AS DepJunio,
					CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    )    AS DepJulio,
					CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    )    AS DepAgosto,
					CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    )    AS DepSeptiembre,
					CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    )    AS DepOctubre,
					CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    )    AS DepNoviembre,
					CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    )    AS DepDiciembre,

					CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    ) * 12 AS DelPeriodo, 

					( ((YEAR(acvo_InicioDepreciacion) - YEAR(GETDATE() ))*(-1.00)) * (CONVERT(DECIMAL(18,2),((acvo_CostoDespreciable) / vitu.viut_VidaUtil)) ) ) + ((CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    ) ) * 12)  AS ACUMULADA,

					acvo_CostoOriginal - ( ( ((YEAR(acvo_InicioDepreciacion) - YEAR(GETDATE() ))*(-1.00)) * (CONVERT(DECIMAL(18,2),((acvo_CostoDespreciable) / vitu.viut_VidaUtil)) ) ) + ((CONVERT(DECIMAL(18,2),(  ROUND( ( (acvo_CostoOriginal - acvo_ValorResidual) /   CONVERT(DECIMAL(18,2),(vitu.viut_VidaUtil)) / 12  ),2)    )    ) ) * 12) ) AS ValorLibro

		 FROM		Acti.tbActivos acvo LEFT JOIN Acti.tbVidaUtil vitu
		 ON			acvo.viut_Id = vitu.viut_Id LEFT JOIN Gral.tbClientes clie
		 ON			acvo.clie_Id = clie.clie_Id
		 WHERE		acvo.clie_Id = @clie_Id AND acvo_Estado = @acvo_Estado
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH
 END

GO

CREATE OR ALTER PROCEDURE Acti.UDP_tbActivos_ReporteFechas 
	@clie_Id		INT,
	@FechaInicio	DATE,
	@FechaFin		DATE
AS
BEGIN
	BEGIN TRY
		SELECT	 acvo_Nombre, 
				 acvo_NumeroSerie, 
				 util.viut_Objeto,
				 util.viut_VidaUtil,
				 acvo_Ubicacion, 
				 acvo_FechaAdquisicion, 
				 acvo_InicioDepreciacion, 
				 acvo_CostoOriginal,
				 acvo_ValorResidual,
				 acvo_FechaCreacion,
				 acvo_CostoDespreciable
		FROM     Acti.tbActivos acti INNER JOIN Acti.tbVidaUtil util
		ON		 acti.viut_Id = util.viut_Id  
		WHERE	 acti.acvo_FechaCreacion BETWEEN @FechaInicio AND @FechaFin
	END TRY
	BEGIN CATCH
		SELECT 0
	END CATCH
END

GO

CREATE OR ALTER PROCEDURE Acti.UDP_tbActivos_VerificarActivosCliente 
	@clie_Id		INT
AS
BEGIN
	BEGIN TRY
		IF EXISTS(SELECT acvo_Id FROM Acti.tbActivos WHERE clie_Id = @clie_Id AND acvo_Estado = 0)
			BEGIN
				SELECT 1
			END
		ELSE
			BEGIN
				SELECT 0
			END
	END TRY
	BEGIN CATCH
		SELECT -1
	END CATCH
END

GO


CREATE OR ALTER PROCEDURE Acti.UDP_tbActivos_VerificarActivosNoDadosBajaCliente
	@clie_Id		INT
AS
BEGIN
	BEGIN TRY
		IF EXISTS(SELECT acvo_Id FROM Acti.tbActivos WHERE clie_Id = @clie_Id AND acvo_Estado = 1)
			BEGIN
				SELECT 1
			END
		ELSE
			BEGIN
				SELECT 0
			END
	END TRY
	BEGIN CATCH
		SELECT -1
	END CATCH
END

GO
 
CREATE OR ALTER PROCEDURE Acti.UDP_tbActivos_VerificarActivosTotalesCliente 
	@clie_Id		INT
AS
BEGIN
	BEGIN TRY
		IF EXISTS(SELECT acvo_Id FROM Acti.tbActivos WHERE clie_Id = @clie_Id AND (acvo_Estado = 1 OR acvo_Estado = 0))
			BEGIN
				SELECT 1
			END
		ELSE
			BEGIN
				SELECT 0
			END
	END TRY
	BEGIN CATCH
		SELECT -1
	END CATCH
END

GO

CREATE OR ALTER PROCEDURE Acti.UDP_tbClientes_ClienteConActivosBajados
AS
BEGIN
	BEGIN TRY
	  DECLARE @TotalActivos INT;

    SELECT @TotalActivos = COUNT(acvo.acvo_Id)
    FROM Acti.tbActivos acvo
    WHERE acvo.acvo_Estado = 0;

    IF @TotalActivos = 0
    BEGIN
        SELECT 'No hay cliente' AS clie_NombreCliente,
               'N/A' AS clie_CodigoCompania,
               0 AS TotalActivos;
    END
    ELSE
    BEGIN
       SELECT TOP 1
			clie.clie_NombreCliente,
			clie.clie_CodigoCompania,
			COUNT(acto.acvo_Id) AS TotalActivos
			FROM Gral.tbClientes clie
			LEFT JOIN Acti.tbActivos acto
			ON clie.clie_Id = acto.clie_Id
			WHERE acto.acvo_Estado = 0
			GROUP BY clie.clie_Id, clie.clie_NombreCliente, clie.clie_CodigoCompania
			ORDER BY TotalActivos DESC;
    END
	END TRY
	BEGIN CATCH

	END CATCH
END

GO

CREATE OR ALTER PROCEDURE Acti.UDP_tbClientes_ClienteConActivosNoBajados
AS
BEGIN
    DECLARE @TotalActivos INT;

    SELECT @TotalActivos = COUNT(acvo.acvo_Id)
    FROM Acti.tbActivos acvo
    WHERE acvo.acvo_Estado = 1;

    IF @TotalActivos = 0
    BEGIN
        SELECT 'No hay cliente' AS clie_NombreCliente,
               'N/A' AS clie_CodigoCompania,
               0 AS TotalActivos;
    END
    ELSE
    BEGIN
       SELECT TOP 1
			clie.clie_NombreCliente,
			clie.clie_CodigoCompania,
			COUNT(acto.acvo_Id) AS TotalActivos
			FROM Gral.tbClientes clie
			LEFT JOIN Acti.tbActivos acto
			ON clie.clie_Id = acto.clie_Id
			WHERE acto.acvo_Estado = 1
			GROUP BY clie.clie_Id, clie.clie_NombreCliente, clie.clie_CodigoCompania
			ORDER BY TotalActivos DESC;
    END
END
	
	
GO

CREATE OR ALTER PROCEDURE Acti.UDP_tbClientes_CantidadDeClientes
AS
BEGIN
	SELECT	COUNT(clie.clie_Id) AS clie_Id
	FROM Gral.tbClientes clie
END

GO

CREATE OR ALTER PROCEDURE Acti.UDP_tbVidaUtil_VidaUtilMasUsadas
AS
BEGIN
	SELECT	util.viut_Objeto,
			util.viut_VidaUtil,
			util.viut_Descripcion,
			COUNT(acvo.viut_Id) as CantidadUsadas,
			CONCAT(
				CAST(ROUND((COUNT(acvo.viut_Id) * 100.0) / SUM(COUNT(acvo.viut_Id)) OVER (), 2) AS DECIMAL(10, 2)),
				'%'
			) as Porcentaje
	FROM    Acti.tbVidaUtil util INNER JOIN Acti.tbActivos acvo
	ON		util.viut_Id = acvo.viut_Id
	GROUP BY util.viut_Objeto,util.viut_VidaUtil,util.viut_Descripcion
	ORDER BY util.viut_Objeto,util.viut_VidaUtil,util.viut_Descripcion
END

