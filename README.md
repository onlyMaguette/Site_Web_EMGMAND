# Exécution de l'application EMGMAND en local

Cette application est un projet web basé sur .NET et utilise une base de données MySQL/MariaDB. L'application utilise également **JWT** pour l'authentification. Ce fichier explique comment exécuter l'application en local pour le développement et le test.

## Prérequis

Avant d'exécuter l'application, vous devez avoir installé les éléments suivants sur votre machine :

- [.NET SDK](https://dotnet.microsoft.com/download/dotnet) (version 6.0 ou plus récente)
- [MySQL ou MariaDB](https://dev.mysql.com/downloads/installer/) (ou un autre système de gestion de base de données compatible avec MySQL)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/) ou un autre éditeur de code compatible avec .NET (comme VS Code)
- [XAMPP ou WAMP](https://www.apachefriends.org/) si vous utilisez Windows pour gérer MySQL facilement

## Étapes pour exécuter l'application en local

### 1. Cloner le dépôt

Clonez ce projet depuis GitHub (ou le repository de votre choix) avec la commande suivante :

```bash
git clone https://github.com/onlyMaguette/Site_Web_EMGMAND.git
```

### 2. Démarrer le serveur MySQL

Avant de configurer la base de données, assurez-vous que votre **serveur MySQL** est en cours d'exécution. Vous pouvez vérifier son statut et le démarrer si nécessaire :

#### Sous Windows (XAMPP) :

- Ouvrez le panneau de configuration XAMPP et cliquez sur **Start** pour MySQL.

#### Sous Linux/Mac :

```bash
sudo systemctl start mysql  # Pour les distributions utilisant systemd
sudo service mysql start     # Alternative pour certaines distributions
```

Vous pouvez vérifier que MySQL fonctionne avec la commande suivante :

```bash
mysqladmin -u root -p status
```

### 3. Configurer la base de données

Une fois que votre serveur MySQL est en cours d'exécution, créez une base de données pour l'application. Vous pouvez le faire en utilisant MySQL CLI ou un autre outil de gestion de base de données :

```sql
CREATE DATABASE emgmand;
```

Ensuite, dans le fichier `appsettings.json` de votre projet, configurez la chaîne de connexion à la base de données. Exemple :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=emgmand;User=root;Password=yourpassword;"
  },
  "Jwt": {
    "Issuer": "http://localhost:5279",
    "Audience": "http://localhost:5279",
    "SecretKey": "a17b2d11ad5d4331aad0e088d98aa619"
  }
}
```

### 4. Générer la clé secrète JWT

Pour une meilleure sécurité, vous devez générer une nouvelle clé secrète pour le JWT. Vous pouvez générer une clé secrète aléatoire en utilisant une commande comme celle-ci :

#### PowerShell :

```powershell
[guid]::NewGuid().ToString("N")
```

#### Linux/Mac :

```bash
uuidgen | tr -d '-'
```

Remplacez ensuite la valeur de `"SecretKey"` dans `appsettings.json` par la clé générée.

### 5. Restaurer les dépendances

Assurez-vous que toutes les dépendances du projet sont installées en exécutant la commande suivante à la racine du projet :

```bash
dotnet restore
```

### 6. Appliquer les migrations de la base de données

Si votre projet utilise Entity Framework pour gérer la base de données, appliquez les migrations pour créer les tables dans la base de données MySQL :

```bash
dotnet ef database update
```

### 7. Exécuter l'application

Avant d'exécuter l'application, assurez-vous d'être dans le dossier contenant la solution EMGMAND.sln :

```bash
cd EMGMAND
```

Ensuite, démarrez l'application avec :

```bash
dotnet run
```

Cela démarrera l'application sur le port `5279` (ou un autre port attribué par le système), et vous devriez voir un message indiquant que l'application est maintenant en écoute :

```
Now listening on: http://localhost:5279
```

### 8. Accéder à l'application

Vous pouvez maintenant accéder à l'application en ouvrant un navigateur et en naviguant à l'adresse suivante :

```
http://localhost:5279
```

### 9. Tester l'authentification

L'application utilise **JWT** pour l'authentification. Lors de l'authentification, un **token JWT** sera généré et utilisé pour valider les utilisateurs. Vous pouvez tester les points d'API sécurisés avec un outil comme [Postman](https://www.postman.com/) ou `HttpClient`.

### 10. Débogage et logs

Si vous rencontrez des erreurs, consultez les logs dans la console. En mode développement, les erreurs seront affichées en détail.

### 11. Arrêter l'application

Pour arrêter l'application, appuyez sur **Ctrl + C** dans la console.

## Informations supplémentaires
- Assurez-vous que votre base de données MySQL/MariaDB est bien sécurisée et que les informations de connexion ne sont pas exposées publiquement.
