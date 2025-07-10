CREATE PROCEDURE UpdateStatesOrder
    @Id1 INTEGER,
    @Order1 INTEGER,
    @Id2 INTEGER,
    @Order2 INTEGER,
    @BoardId UNIQUEIDENTIFIER
AS
BEGIN
    UPDATE States
    SET [Order] = CASE 
        WHEN Id = @Id1 THEN @Order1
        WHEN Id = @Id2 THEN @Order2
        ELSE [Order]
    END
    WHERE Id IN (@Id1, @Id2) AND BoardId = @BoardId;
END;