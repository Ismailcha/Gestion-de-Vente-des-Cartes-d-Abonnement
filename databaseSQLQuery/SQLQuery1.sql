create database GestionVenteCarteAbonnement;
go
use GestionVenteCarteAbonnement;
go
create table Employe(
matricule int constraint PK_Utilisateur primary key identity(2939,7),
nom varchar(20) not null,
prenom varchar(20) not null,
email varchar(50) constraint UQ_email unique not null,
motPasse varchar(50) not null);
go
create table Client(
codeClient int constraint PK_codeClient primary key identity(923,3),
nom varchar(20) not null,
prenom varchar(20) not null,
dateNaissance date not null,
numCIN varchar(13) constraint UQ_numEtudiant unique,
);
go
create table Carte(
codeCarte int constraint PK_Carte primary key identity(8367,41),
dateInscription date,
dateExpiration date,
typeCarte varchar(20) not null,
gareDepart varchar(50),
gareArrive varchar(50),
codeClient int constraint FK_Client_Carte foreign key references Client,
matricule int constraint FK_Employe_Carte foreign key references Employe
);
go
--------------------------------------------------------------------------------
create trigger tr_User_Password
on Employe
for insert
as
declare @motPasseInserted varchar(50) = (select motPasse from inserted)
update Employe
set motPasse=hashbytes('md5',@motPasseInserted);
go
--------------------------------------------------------------------------------
create or alter proc sp_verify_user
@email varchar(50),
@motPasse varchar(50)
as begin
declare @emails varchar(50)= (select email from Employe)
if(@email in (@emails))
begin
if(hashbytes('md5',@motPasse)=(select motPasse from Employe where email=@email))
begin
return 1
end
end
else
begin 
return 0
end
end;
go

create or alter proc sp_verify_client
@CIN varchar(13)
as begin
if(@CIN in (select numCIN from Client))
begin 
return 1
end
end;
go

create or alter proc sp_add_Client
@nom varchar(20),
@prenom varchar(20),
@dateNaiss date,
@numCIN varchar(13)
as begin
insert into Client values (@nom,@prenom,@dateNaiss,@numCIN)
end;
go

create or alter proc sp_add_card
@numCIN varchar(13),
@email varchar(50),
@dateExpiration date,
@typeCarte varchar(13),
@gareDepart varchar(50) = NULL,
@gareArrivee varchar(50) = NULL
as begin
declare @codeClt int=(select codeClient from Client where numCIN = @numCIN)
declare @matricule int = (select matricule from Employe where email = @email)
if(@typeCarte='Abonnement')
insert into Carte values (getDate(),@dateExpiration,@typeCarte,@gareDepart,@gareArrivee,@codeClt,@matricule)
else
insert into Carte values (getDate(),@dateExpiration,@typeCarte,null,null,@codeclt,@matricule)
end;
go
---2eme version sp_add_card
create or alter proc sp_add_card
@numCIN varchar(13),
@email varchar(50),
@typeCarte varchar(13),
@gareDepart varchar(50) = NULL,
@gareArrivee varchar(50) = NULL
as begin
declare @codeClt int=(select codeClient from Client where numCIN = @numCIN)
declare @matricule int = (select matricule from Employe where email = @email)
if(@typeCarte='Abonnement')
insert into Carte values (getDate(),DATEADD(m,1,getdate()),@typeCarte,@gareDepart,@gareArrivee,@codeClt,@matricule)
else
insert into Carte values (getDate(),dateadd(m,1,getdate()),@typeCarte,null,null,@codeclt,@matricule)
end;
go

create or alter proc sp_renew
@codeCarte int,
@dateExpiration date
as begin
update Carte
set dateExpiration=@dateExpiration
where @codeCarte = codeCarte
end;
go

create or alter proc sp_verify_email
@email varchar(50)
as begin
if not exists(select email from employe where email = @email)
begin
return 1
end
else
begin
return 0
end
end;
go

create or alter proc sp_add_employe
@nom varchar(20),
@prenom varchar(20),
@email varchar(50),
@motPasse varchar(50)
as begin
insert into employe values (@nom,@prenom,@email,@motPasse)
end;
go

create or alter proc sp_get_user
@email varchar(50)
as begin
select nom + ' ' + prenom as EmployeName from employe where email = @email
end;
go

create or alter proc sp_Rechercher_Code
@codeCarte int
as begin
select codeCarte,Nom, Prenom,numCIN, typeCarte,dateExpiration
from client inner join carte on client.codeClient = carte.codeClient
where @codeCarte = codeCarte
end;
go

create or alter proc sp_rechercher_CIN
@numCIN varchar(13)
as begin
select codeCarte,Nom,Prenom,numCIN,TypeCarte,dateExpiration
from client inner join carte on client.codeClient = carte.CodeCarte
where @numCIN = numCIN
end;
go

create or alter proc sp_rechercher_Nom
@nom varchar(20)
as begin
select codeCarte,Nom,Prenom,numCIN,TypeCarte,dateExpiration
from client inner join carte on client.codeClient = carte.CodeCarte
where @nom = nom
end;
go

create or alter proc sp_supprimer_Client
@codeCarte int,
@numCIN varchar(13)
as begin
delete from carte where @codeCarte = codeCarte
delete from client where @numCIN = numCIN
end;
go