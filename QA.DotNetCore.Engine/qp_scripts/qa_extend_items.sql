ALTER PROCEDURE [dbo].[qa_extend_items]
	@ContentId int, -- ID контента расширения
	@IsLive bit = 1,
	@Ids ListOfIds readonly,
	@includeBaseFields bit = 0 -- догружать ли поля основной статьи
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @baseContentId int = 0
	IF (@includeBaseFields = 1)
	BEGIN
		SELECT @baseContentId = CONTENT_ID
			FROM [CONTENT]
			WHERE NET_CONTENT_NAME = 'QPAbstractItem' AND SITE_ID = (SELECT TOP 1 SITE_ID FROM [CONTENT] WHERE CONTENT_ID = @ContentId)
	END
	DECLARE @isSplitted AS BIT = NULL	
	DECLARE @tablesuffix NVARCHAR(255) = N''
	DECLARE @tablename NVARCHAR(255) = N'CONTENT_' + CAST(@contentId AS NVARCHAR(255))
	

	IF (@isSplitted IS NULL)
	BEGIN
		SELECT TOP 1 @isSplitted = CONTENT.USE_DEFAULT_FILTRATION 
			FROM dbo.[CONTENT]
				WHERE CONTENT.CONTENT_ID = @contentid
	END

	IF (@isSplitted = 1)
	BEGIN
		IF (@isLive = 1)
			SET @tablesuffix = '_LIVE_NEW'
		ELSE
			SET @tablesuffix = '_STAGE_NEW'	
	END

	SET @tablename = @tablename + @tablesuffix

	DECLARE @query NVARCHAR(MAX) = N'
			SELECT 
			* 
			FROM ' + @tablename + ' ext with (nolock)
			INNER JOIN @x on Id = ext.itemid
			'
	SET @tablesuffix = ''
	IF(@includeBaseFields = 1 AND @baseContentId > 0)
	BEGIN
		SELECT TOP 1 @isSplitted = CONTENT.USE_DEFAULT_FILTRATION 
			FROM dbo.[CONTENT]
				WHERE CONTENT.CONTENT_ID = @baseContentId

		IF (@isSplitted = 1)
		BEGIN
			IF (@isLive = 1)
				SET @tablesuffix = '_LIVE_NEW'
			ELSE
				SET @tablesuffix = '_STAGE_NEW'	
		END

		DECLARE @abstractItemViewName NVARCHAR(255) = N'CONTENT_' + CAST(@baseContentId AS NVARCHAR(255)) + @tablesuffix

		set @query = @query + N' INNER JOIN ' + @abstractItemViewName + ' ai on ai.Content_item_id = ext.itemid'

	END

		
	--print @query

	exec dbo.SP_EXECUTESQL @query, N'@x ListOfIds READONLY', @x = @ids;
	
END
