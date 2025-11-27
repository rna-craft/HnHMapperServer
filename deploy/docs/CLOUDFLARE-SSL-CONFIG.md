# Cloudflare SSL/TLS Configuration Guide

## Current Setup Status: ✅ WORKING

Your HnH Mapper deployment is now successfully accessible at:
- **HTTP**: http://your.domain.com (redirects to HTTPS)
- **HTTPS**: https://your.domain.com (fully working)

## SSL/TLS Mode Configuration

Since you're using Cloudflare proxy (orange cloud) with Caddy, you need to configure the correct SSL/TLS mode in Cloudflare.

### Recommended Mode: Full (strict)

**Path**: Cloudflare Dashboard → SSL/TLS → Overview → Encryption mode

Set to: **Full (strict)**

### Why Full (strict)?

- **End-to-end encryption**: Traffic encrypted both client→Cloudflare AND Cloudflare→your server
- **Certificate validation**: Cloudflare validates your origin certificate (Let's Encrypt from Caddy)
- **Best security**: Prevents man-in-the-middle attacks between Cloudflare and your server

### SSL/TLS Mode Options Explained

| Mode | Description | Use Case | Security |
|------|-------------|----------|----------|
| **Off** | No HTTPS, HTTP only | Never use | ❌ Insecure |
| **Flexible** | HTTPS to Cloudflare, HTTP to origin | Legacy servers without SSL | ⚠️ Partial encryption |
| **Full** | HTTPS to Cloudflare, HTTPS to origin (any cert) | Self-signed certs on origin | ⚠️ No cert validation |
| **Full (strict)** | HTTPS to Cloudflare, HTTPS to origin (valid cert) | **Your setup** | ✅ Secure |
| **Strict (SSL-Only Origin Pull)** | Requires Cloudflare Origin CA cert | Advanced setups | ✅ Very Secure |

### Certificate Chain

With your current setup:

1. **Client → Cloudflare**:
   - Certificate: Cloudflare Universal SSL (issued by Google Trust Services)
   - Subject: rna-craft.com
   - Valid: Nov 18, 2025 - Feb 16, 2026

2. **Cloudflare → Your Server**:
   - Certificate: Let's Encrypt (via Caddy DNS-01 challenge)
   - Subject: your.domain.com
   - Issued by: Let's Encrypt
   - Auto-renewed by Caddy every 60 days

## Additional Cloudflare Settings

### 1. Always Use HTTPS

**Path**: SSL/TLS → Edge Certificates → Always Use HTTPS

**Set to**: ON

This ensures all HTTP requests are redirected to HTTPS (redundant with Caddy's redirect, but adds defense-in-depth).

### 2. Minimum TLS Version

**Path**: SSL/TLS → Edge Certificates → Minimum TLS Version

**Recommended**: TLS 1.2 (or TLS 1.3 for maximum security)

### 3. Automatic HTTPS Rewrites

**Path**: SSL/TLS → Edge Certificates → Automatic HTTPS Rewrites

**Set to**: ON

Rewrites HTTP URLs to HTTPS in HTML content.

### 4. HTTP Strict Transport Security (HSTS)

**Path**: SSL/TLS → Edge Certificates → HSTS

**Recommended settings**:
- Enable HSTS: ON
- Max Age Header: 6 months (15768000 seconds)
- Include subdomains: ON (if applicable)
- Preload: ON (optional, requires submitting to HSTS preload list)

⚠️ **Warning**: Only enable HSTS if you're confident HTTPS will always work. It forces browsers to only use HTTPS.

## Troubleshooting

### "Too Many Redirects" Error

**Cause**: Cloudflare SSL mode set to "Flexible" while Caddy redirects HTTP to HTTPS

**Solution**: Change Cloudflare SSL mode to "Full" or "Full (strict)"

### Certificate Errors

**Cause**: Cloudflare SSL mode set to "Full (strict)" but origin cert is invalid/expired

**Solution**:
1. Check Caddy logs: `docker compose logs caddy | grep certificate`
2. Verify Let's Encrypt cert is valid: `docker compose exec caddy caddy list-certificates`
3. Ensure Cloudflare API token has DNS edit permissions

### 526 Error (Invalid SSL Certificate)

**Cause**: Cloudflare can't validate your origin certificate

**Solution**:
1. Verify Caddy obtained a certificate successfully (check logs)
2. Change Cloudflare SSL mode from "Full (strict)" to "Full" temporarily
3. Debug certificate issue, then switch back to "Full (strict)"

## Verification Commands

Test your site's SSL configuration:

```bash
# Check HTTP redirect
curl -I http://your.domain.com

# Check HTTPS response
curl -I https://your.domain.com

# View certificate chain
echo | openssl s_client -servername your.domain.com -connect your.domain.com:443 2>/dev/null | openssl x509 -noout -text

# Test SSL Labs (comprehensive SSL analysis)
# Visit: https://www.ssllabs.com/ssltest/analyze.html?d=your.domain.com
```

## Security Best Practices

1. ✅ **Use Full (strict) SSL mode** - End-to-end encryption with cert validation
2. ✅ **Enable HSTS** - Force HTTPS in browsers
3. ✅ **Use TLS 1.2+ minimum** - Disable older, insecure protocols
4. ✅ **Enable Automatic HTTPS Rewrites** - Fix mixed content
5. ✅ **Monitor certificate expiration** - Caddy auto-renews, but watch for issues
6. ✅ **Keep Cloudflare API token secure** - Store in .env, never commit to git
7. ✅ **Backup certificate volume** - Preserve `caddy_data` volume

## Current Status Summary

Your deployment is **fully operational** with:
- ✅ Let's Encrypt certificate obtained via Cloudflare DNS-01
- ✅ Caddy serving HTTPS on port 443
- ✅ Cloudflare proxy enabled (orange cloud)
- ✅ End-to-end encryption configured
- ✅ All security headers enabled
- ✅ HTTP→HTTPS redirect working
- ✅ OCI firewall rules configured (ports 80, 443)
- ✅ iptables rules configured

**No further action required** - your site is secure and accessible!
