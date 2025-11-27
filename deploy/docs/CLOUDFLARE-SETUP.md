# Cloudflare DNS-01 ACME Setup Guide

This guide explains how to configure the HnH Mapper Server to use Cloudflare DNS-01 ACME challenges for automatic HTTPS certificate provisioning.

## Benefits of Cloudflare DNS-01

- **Wildcard certificates**: Supports `*.yourdomain.com` certificates
- **No port exposure required**: Works even if ports 80/443 aren't publicly accessible
- **Behind NAT/firewall**: Perfect for servers behind Cloudflare proxy
- **Rate limit friendly**: More reliable than HTTP-01 challenges

## Prerequisites

1. **Domain managed by Cloudflare**: Your domain's nameservers must point to Cloudflare
2. **Cloudflare account**: Free tier is sufficient
3. **API token with DNS edit permissions**: Created in Cloudflare dashboard

## Setup Instructions

### Step 1: Create Cloudflare API Token

1. Log in to [Cloudflare Dashboard](https://dash.cloudflare.com/)
2. Navigate to **My Profile → API Tokens**
3. Click **Create Token**
4. Use the **Edit zone DNS** template
5. Configure token permissions:
   - **Permissions**: Zone → DNS → Edit
   - **Zone Resources**: Include → Specific zone → `yourdomain.com`
6. Click **Continue to summary** → **Create Token**
7. **Copy the token** (you won't be able to see it again!)

### Step 2: Configure Environment Variables

1. Copy the environment template:
   ```bash
   cd deploy
   cp .env.example .env
   ```

2. Edit `.env` and set your values:
   ```bash
   # Your domain name
   CADDY_DOMAIN=yourdomain.com

   # Cloudflare API token (from Step 1)
   CLOUDFLARE_API_TOKEN=your-cloudflare-api-token-here

   # Email for Let's Encrypt notifications
   CLOUDFLARE_EMAIL=admin@yourdomain.com

   # GitHub Container Registry upstream (for Watchtower)
   UPSTREAM=your-github-username
   ```

3. **Secure the .env file** (contains sensitive credentials):
   ```bash
   chmod 600 .env
   ```

### Step 3: Deploy with Docker Compose

1. Build the custom Caddy image with Cloudflare DNS plugin:
   ```bash
   docker compose build caddy
   ```

2. Start all services:
   ```bash
   docker compose up -d
   ```

3. Monitor Caddy logs to verify certificate issuance:
   ```bash
   docker compose logs -f caddy
   ```

   You should see:
   ```
   [INFO] [yourdomain.com] Obtaining certificate
   [INFO] [yourdomain.com] Using DNS challenge solver
   [INFO] [yourdomain.com] Certificate obtained successfully
   ```

### Step 4: Verify HTTPS

1. Visit your domain: `https://yourdomain.com`
2. Check certificate details in browser (should show Let's Encrypt)
3. Verify automatic HTTP→HTTPS redirect works

## Configuration Files

### Caddyfile

The Caddyfile has been updated with:

```caddyfile
# Global options for Cloudflare DNS-01 ACME challenge
{
  email {$CLOUDFLARE_EMAIL}
}

# Domain configuration with Cloudflare DNS
${env.CADDY_DOMAIN} {
  tls {
    dns cloudflare {$CLOUDFLARE_API_TOKEN}
  }

  # ... routing rules ...
}
```

### docker-compose.yml

The Caddy service now:
- Builds from `docker/caddy-cloudflare.Dockerfile` (includes Cloudflare DNS plugin)
- Receives environment variables from `.env` file
- Persists certificates in `caddy_data` volume

## Troubleshooting

### Certificate Request Fails

**Error**: `failed to get certificate`

**Solutions**:
1. Verify API token has DNS edit permissions
2. Check domain is managed by Cloudflare (nameservers set correctly)
3. Ensure `CADDY_DOMAIN` matches your actual domain
4. Check Caddy logs: `docker compose logs caddy`

### API Token Invalid

**Error**: `cloudflare: invalid API token`

**Solutions**:
1. Regenerate API token in Cloudflare dashboard
2. Ensure token has **Zone → DNS → Edit** permission
3. Verify token is for the correct zone (domain)
4. Update `CLOUDFLARE_API_TOKEN` in `.env` and restart:
   ```bash
   docker compose restart caddy
   ```

### Rate Limited by Let's Encrypt

**Error**: `too many certificates already issued`

**Solution**: Use Let's Encrypt staging for testing:
1. Edit `Caddyfile`, uncomment the staging line:
   ```caddyfile
   {
     email {$CLOUDFLARE_EMAIL}
     acme_ca https://acme-staging-v02.api.letsencrypt.org/directory
   }
   ```
2. Restart Caddy: `docker compose restart caddy`
3. Test your configuration with staging certificates (browsers will show warning)
4. Once working, comment out `acme_ca` line and restart for production certificates

### DNS Propagation Issues

**Error**: `DNS problem: NXDOMAIN looking up TXT record`

**Solutions**:
1. Wait 5-10 minutes for DNS propagation
2. Verify DNS record exists in Cloudflare dashboard
3. Test DNS resolution: `dig TXT _acme-challenge.yourdomain.com`

## Testing with Staging Certificates

To avoid Let's Encrypt rate limits during testing:

1. Edit `deploy/Caddyfile`, uncomment:
   ```caddyfile
   acme_ca https://acme-staging-v02.api.letsencrypt.org/directory
   ```

2. Restart Caddy:
   ```bash
   docker compose restart caddy
   ```

3. Test your setup (browser will show certificate warning - this is expected)

4. Once confirmed working, comment out `acme_ca` and restart for production certs

## Wildcard Certificates (Optional)

To use wildcard certificates for subdomains:

1. Edit `deploy/Caddyfile`, change domain to:
   ```caddyfile
   *.yourdomain.com, yourdomain.com {
     tls {
       dns cloudflare {$CLOUDFLARE_API_TOKEN}
     }
     # ... routing rules ...
   }
   ```

2. Restart Caddy:
   ```bash
   docker compose restart caddy
   ```

## Security Best Practices

1. **Never commit `.env` to version control** (already in `.gitignore`)
2. **Restrict API token permissions**: Only grant DNS edit for specific zone
3. **Rotate API tokens periodically**: Create new token, update `.env`, delete old token
4. **Monitor certificate expiration**: Check Cloudflare email for Let's Encrypt notifications
5. **Backup certificate volume**: Preserve `caddy_data` volume across deployments

## Certificate Renewal

Caddy automatically renews certificates 30 days before expiration. No manual intervention required.

To force renewal (for testing):
```bash
docker compose exec caddy caddy reload --config /etc/caddy/Caddyfile
```

## Additional Resources

- [Caddy Cloudflare DNS Plugin](https://github.com/caddy-dns/cloudflare)
- [Caddy Automatic HTTPS](https://caddyserver.com/docs/automatic-https)
- [Cloudflare API Tokens](https://developers.cloudflare.com/fundamentals/api/get-started/create-token/)
- [Let's Encrypt Rate Limits](https://letsencrypt.org/docs/rate-limits/)

## Support

If you encounter issues:
1. Check Caddy logs: `docker compose logs caddy`
2. Verify environment variables: `docker compose config`
3. Test API token in Cloudflare dashboard
4. Review this guide's troubleshooting section
