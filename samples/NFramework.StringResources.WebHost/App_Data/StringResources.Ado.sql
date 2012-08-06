/*

파일명 : StringResources.Ado.sql

설명 : 
	Application 환경에서의 String Resource 정보를 Database에서 제공하기 위해 
	Resource 정보를 DB Table에 저장해 둔다.
C:\RealWeb Products\RCL\RCL-NET\RCL-Globalization\RCL.StringResources.WebApp\App_Data\StringResources.Ado.sql
	제품별로 Resource를 관리할 수 있다. 
	만약 회사마다 구분하고 싶다면 TABLE을 따로 만들어서 복사하는 것이 좋을 것이다.	

만든이 : 배성혁 (debop@realweb21.com)


개발이력 :

	2009-12-22 : AssemblyName column 추가 (제품 또는 모듈 구분을 위해)
	2007-02-23 : 최초 작성
	
*/

use StringResourcesDB
GO

IF OBJECT_ID('StringResources') <> 0 
   DROP TABLE StringResources
GO

CREATE TABLE dbo.StringResources
	   (
		AssemblyName NVARCHAR(256) NOT NULL DEFAULT('DEFAULT')
	  , ResourceName NVARCHAR(256) NOT NULL
	  , LocaleKey NVARCHAR(256) NOT NULL DEFAULT (N'ko')
	  , ResourceKey NVARCHAR(256) NOT NULL
	  , ResourceValue NVARCHAR(MAX)
			CONSTRAINT [PK_StringResources]
			PRIMARY KEY CLUSTERED (AssemblyName ASC, ResourceName ASC, LocaleKey ASC, ResourceKey ASC) WITH (IGNORE_DUP_KEY=OFF) ON [PRIMARY]
	   )
ON     [PRIMARY]

GO

--- 여기부터는 Sample Code 이므로 실제 적용시에는 없애야 한다.

insert into StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) values ('Glossary', 'en', 'HomePage', 'HomePage (en)')
insert into StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) values ('Glossary', 'ko', 'HomePage', '홈페이지 (ko)')
insert into StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) values ('Glossary', 'ko-KR', 'HomePage', '홈페이지 (ko-KR)')


insert into StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) values ('Glossary', 'en', 'SiteMap', 'Site Map (en)')
insert into StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) values ('Glossary', 'ko', 'SiteMap', '사이트 맵 (ko-KR)')


INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Glossary', 'en', 'Welcome','Welcome to ${HomePage} (en)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Glossary', 'en-US', 'Welcome','Welcome to ${HomePage} (en-US)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Glossary', 'en-CA', 'Welcome','Welcome to ${HomePage} (en-CA)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Glossary', 'ko', 'Welcome','${HomePage}에 오신 것을 환영합니다. (ko)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Glossary', 'ko-KR', 'Welcome',' ${HomePage}에 오신 것을 환영합니다. (ko-KR)')

INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Config', 'en','FlagImage','')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Config', 'en-CA','FlagImage','~/images/flag.en-CA.gif')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Config', 'en-US','FlagImage','~/images/flag.en-US.gif')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Config', 'ko','FlagImage','')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Config', 'ko-KR','FlagImage','~/images/flag.ko-KR.gif')

INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('CommonTerms', 'en','Hello','Hello, ${Glossary|Welcome} - Introducing (en)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('CommonTerms', 'en-CA','Hello','Hello, ${Glossary|Welcome} - Introducing (en-CA)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('CommonTerms', 'en-US','Hello','Hello, ${Glossary|Welcome} - Introducing (en-US)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('CommonTerms', 'ko','Hello','안녕하세요 ${Glossary|Welcome} - 인사말이었습니다. (ko)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('CommonTerms', 'ko-KR','Hello','안녕하세요. ${Glossary|Welcome} - 인사말이었습니다. (ko-KR)')

INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Expressions.aspx', 'en','lblHelloLocalResource1.Text','Hello (en)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue)  Values ('Expressions.aspx', 'en-US','lblHelloLocalResource1.Text','Hello (en-US)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Expressions.aspx', 'en-CA','lblHelloLocalResource1.Text','Hello (en-CA)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Expressions.aspx', 'ko','lblHelloLocalResource1.Text','안녕하세요 (ko)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('Expressions.aspx', 'ko-KR','lblHelloLocalResource1.Text','안녕하세요 (ko-KR)')

INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('RuntimeCode.aspx', 'en','lblHelloLocalResource1.Text','Hello (en)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('RuntimeCode.aspx', 'en-US','lblHelloLocalResource1.Text','Hello (en-US)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('RuntimeCode.aspx', 'en-CA','lblHelloLocalResource1.Text','Hello (en-CA)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('RuntimeCode.aspx', 'ko','lblHelloLocalResource1.Text','안녕하세요 (ko)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('RuntimeCode.aspx', 'ko-KR','lblHelloLocalResource1.Text','안녕하세요 (ko-KR)')

INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('NoCompile.aspx', 'en','lblHelloLocalResource1.Text','Hello (en)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('NoCompile.aspx', 'en-US','lblHelloLocalResource1.Text','Hello (en-US)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('NoCompile.aspx', 'en-CA','lblHelloLocalResource1.Text','Hello (en-CA)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('NoCompile.aspx', 'ko','lblHelloLocalResource1.Text','안녕하세요 (ko)')
INSERT INTO StringResources (ResourceName, LocaleKey, ResourceKey, ResourceValue) Values ('NoCompile.aspx', 'ko-KR','lblHelloLocalResource1.Text','안녕하세요 (ko-KR)')

GO

select * from StringResources